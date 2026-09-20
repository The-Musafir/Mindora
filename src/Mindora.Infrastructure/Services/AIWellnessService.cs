using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mindora.Application.DTOs.AI;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Notifications;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AIWellnessService : IAIWellnessService
    {
        private readonly MindoraDbContext _context;
        private readonly IGeminiService _geminiService;
        private readonly ILogger<AIWellnessService> _logger;
        private readonly INotificationDispatcher _dispatcher;

        public AIWellnessService(
            MindoraDbContext context,
            IGeminiService geminiService,
            ILogger<AIWellnessService> logger,
            INotificationDispatcher dispatcher)
        {
            _context = context;
            _geminiService = geminiService;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        // ============================
        // SESSION MANAGEMENT
        // ============================

        public async Task<AIWellnessCoachSessionDto> CreateSessionAsync(Guid userId, string? initialContext = null)
        {
            var session = new AIWellnessCoachSession
            {
                SessionId = Guid.NewGuid(),
                UserId = userId,
                StartedAt = DateTime.UtcNow,
                IsActive = true,
                LastContext = initialContext
            };

            _context.AIWellnessCoachSessions.Add(session);
            await _context.SaveChangesAsync();

            return MapSessionToDto(session);
        }

        public async Task<AIWellnessCoachSessionDto?> GetSessionByIdAsync(Guid sessionId)
        {
            var session = await _context.AIWellnessCoachSessions
                .Include(s => s.User)
                .Include(s => s.Messages.OrderBy(m => m.SentAt))
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            return session == null ? null : MapSessionToDto(session);
        }

        public async Task<IReadOnlyList<AIWellnessCoachSessionDto>> GetUserSessionsAsync(Guid userId)
        {
            var sessions = await _context.AIWellnessCoachSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.User)
                .Include(s => s.Messages.OrderBy(m => m.SentAt))
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();

            var result = new List<AIWellnessCoachSessionDto>();
            foreach (var session in sessions)
                result.Add(MapSessionToDto(session));

            return result;
        }

        public async Task<bool> EndSessionAsync(Guid sessionId)
        {
            var session = await _context.AIWellnessCoachSessions.FindAsync(sessionId);
            if (session == null) return false;

            session.IsActive = false;
            session.EndedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId, Guid userId)
        {
            var session = await _context.AIWellnessCoachSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId);

            if (session == null) return false;

            // Delete all messages first
            var messages = await _context.AIChatMessages
                .Where(m => m.SessionId == sessionId)
                .ToListAsync();

            if (messages.Any())
                _context.AIChatMessages.RemoveRange(messages);

            // Delete feedbacks if any
            var feedbacks = await _context.AIFeedbacks
                .Where(f => f.SessionId == sessionId)
                .ToListAsync();

            if (feedbacks.Any())
                _context.AIFeedbacks.RemoveRange(feedbacks);

            _context.AIWellnessCoachSessions.Remove(session);
            await _context.SaveChangesAsync();

            _logger.LogInformation("AI session {SessionId} deleted by user {UserId}",
                sessionId, userId);

            return true;
        }

        public async Task<bool> RenameSessionAsync(Guid sessionId, Guid userId, string? newTitle)
        {
            var session = await _context.AIWellnessCoachSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId);

            if (session == null) return false;

            // Trim and cap length; empty → null (auto-derived)
            var trimmed = string.IsNullOrWhiteSpace(newTitle)
                ? null
                : newTitle.Trim();

            if (trimmed != null && trimmed.Length > 100)
                trimmed = trimmed.Substring(0, 100);

            session.CustomTitle = trimmed;
            await _context.SaveChangesAsync();

            return true;
        }

        // ============================
        // MESSAGE / CHAT
        // ============================

        public async Task<AIChatMessageDto> SendMessageAsync(Guid userId, SendMessageRequest request)
        {
            var session = await _context.AIWellnessCoachSessions
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId && s.UserId == userId);

            if (session == null)
                throw new KeyNotFoundException("AI session not found.");

            var userMessage = new AIChatMessage
            {
                MessageId = Guid.NewGuid(),
                SessionId = session.SessionId,
                Sender = "User",
                Content = request.Content,
                SentAt = DateTime.UtcNow,
                Sentiment = AnalyzeSentiment(request.Content),
                MoodScore = EstimateMood(request.Content),
                RiskLevel = AnalyzeRisk(request.Content),
                Intent = DetectIntent(request.Content)
            };
            _context.AIChatMessages.Add(userMessage);

            var aiResponseText = await GenerateAIResponseAsync(request.Content, session.LastContext);

            var aiResponse = new AIChatMessage
            {
                MessageId = Guid.NewGuid(),
                SessionId = session.SessionId,
                Sender = "AI",
                Content = aiResponseText,
                SentAt = DateTime.UtcNow,
                Sentiment = "Neutral",
                MoodScore = null,
                RiskLevel = AnalyzeRisk(request.Content),
                Intent = DetectIntent(request.Content)
            };
            _context.AIChatMessages.Add(aiResponse);

            session.LastContext = $"{request.Content} | {aiResponseText}";

            await _context.SaveChangesAsync();

            // NOTIFICATION: Crisis detection → notify admins
            if (userMessage.RiskLevel == "High")
            {
                await NotifyAdminsOfCrisisAsync(userId, session.SessionId);
            }

            return MapMessageToDto(aiResponse);
        }

        public async Task<IReadOnlyList<AIChatMessageDto>> GetSessionMessagesAsync(Guid sessionId)
        {
            var messages = await _context.AIChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return messages.Select(MapMessageToDto).ToList();
        }

        // ============================
        // PROMPT TEMPLATES
        // ============================

        public async Task<IReadOnlyList<AIPromptTemplateDto>> GetAllPromptTemplatesAsync()
        {
            var templates = await _context.AIPromptTemplates
                .Where(t => t.IsActive)
                .ToListAsync();

            return templates.Select(t => new AIPromptTemplateDto
            {
                TemplateId = t.TemplateId,
                TemplateKey = t.TemplateKey,
                Title = t.Title,
                PromptText = t.PromptText,
                Tone = t.Tone,
                IsActive = t.IsActive
            }).ToList();
        }

        public async Task<Guid> CreatePromptTemplateAsync(AIPromptTemplateDto dto)
        {
            var template = new AIPromptTemplate
            {
                TemplateId = Guid.NewGuid(),
                TemplateKey = dto.TemplateKey,
                Title = dto.Title,
                PromptText = dto.PromptText,
                Tone = dto.Tone,
                IsActive = dto.IsActive
            };

            _context.AIPromptTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template.TemplateId;
        }

        public async Task<bool> TogglePromptTemplateAsync(Guid templateId, bool isActive)
        {
            var template = await _context.AIPromptTemplates.FindAsync(templateId);
            if (template == null) return false;

            template.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // FEEDBACK
        // ============================

        public async Task<Guid> AddFeedbackAsync(Guid userId, CreateAIFeedbackRequest request)
        {
            var feedback = new AIFeedback
            {
                FeedbackId = Guid.NewGuid(),
                SessionId = request.SessionId,
                UserId = userId,
                MessageId = request.MessageId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.AIFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback.FeedbackId;
        }

        public async Task<IReadOnlyList<AIFeedbackDto>> GetSessionFeedbacksAsync(Guid sessionId)
        {
            var feedbacks = await _context.AIFeedbacks
                .Where(f => f.SessionId == sessionId)
                .Include(f => f.User)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return feedbacks.Select(f => new AIFeedbackDto
            {
                FeedbackId = f.FeedbackId,
                SessionId = f.SessionId,
                UserId = f.UserId,
                UserName = f.User?.UserName ?? f.User?.Email ?? string.Empty,
                MessageId = f.MessageId,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        // ============================
        // INTERACTION LOGS
        // ============================

        public async Task<IReadOnlyList<AIInteractionLogDto>> GetSessionLogsAsync(Guid sessionId)
        {
            var logs = await _context.AIInteractionLogs
                .Where(l => l.SessionId == sessionId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(l => new AIInteractionLogDto
            {
                LogId = l.LogId,
                SessionId = l.SessionId,
                TemplateId = l.TemplateId,
                Action = l.Action,
                Metadata = l.Metadata,
                CreatedAt = l.CreatedAt
            }).ToList();
        }

        // ============================
        // WELLNESS SCORE
        // ============================

        public async Task<WellnessScoreDto> CalculateAndSaveWellnessScoreAsync(CalculateWellnessScoreRequest request)
        {
            var wellness = new WellnessScore
            {
                WellnessScoreId = Guid.NewGuid(),
                UserId = request.UserId,
                Score = request.Score,
                Category = request.Category ?? DetermineCategory(request.Score),
                CalculatedAt = DateTime.UtcNow
            };

            _context.WellnessScores.Add(wellness);
            await _context.SaveChangesAsync();

            return new WellnessScoreDto
            {
                WellnessScoreId = wellness.WellnessScoreId,
                UserId = wellness.UserId,
                Score = wellness.Score,
                Category = wellness.Category,
                CalculatedAt = wellness.CalculatedAt
            };
        }

        public async Task<IReadOnlyList<WellnessScoreDto>> GetUserWellnessScoresAsync(Guid userId)
        {
            var scores = await _context.WellnessScores
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CalculatedAt)
                .ToListAsync();

            return scores.Select(w => new WellnessScoreDto
            {
                WellnessScoreId = w.WellnessScoreId,
                UserId = w.UserId,
                Score = w.Score,
                Category = w.Category,
                CalculatedAt = w.CalculatedAt
            }).ToList();
        }

        // ============================
        // RISK PREDICTION
        // ============================

        public async Task<RiskPredictionDto> CreateRiskPredictionAsync(CreateRiskPredictionRequest request)
        {
            var prediction = new RiskPrediction
            {
                RiskPredictionId = Guid.NewGuid(),
                UserId = request.UserId,
                RiskLevel = request.RiskLevel,
                Probability = request.Probability,
                Reason = request.Reason,
                PredictedAt = DateTime.UtcNow
            };

            _context.RiskPredictions.Add(prediction);
            await _context.SaveChangesAsync();

            return new RiskPredictionDto
            {
                RiskPredictionId = prediction.RiskPredictionId,
                UserId = prediction.UserId,
                RiskLevel = prediction.RiskLevel,
                Probability = prediction.Probability,
                Reason = prediction.Reason,
                PredictedAt = prediction.PredictedAt
            };
        }

        public async Task<IReadOnlyList<RiskPredictionDto>> GetUserRiskPredictionsAsync(Guid userId)
        {
            var predictions = await _context.RiskPredictions
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.PredictedAt)
                .ToListAsync();

            return predictions.Select(r => new RiskPredictionDto
            {
                RiskPredictionId = r.RiskPredictionId,
                UserId = r.UserId,
                RiskLevel = r.RiskLevel,
                Probability = r.Probability,
                Reason = r.Reason,
                PredictedAt = r.PredictedAt
            }).ToList();
        }

        // ============================================================
        // NOTIFICATION HELPERS
        // ============================================================

        private async Task NotifyAdminsOfCrisisAsync(Guid userId, Guid sessionId)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var userName = user?.UserName ?? user?.Email ?? "Unknown";

                var adminIds = await GetAdminUserIdsAsync();

                if (adminIds.Count == 0)
                {
                    _logger.LogWarning("No admin users found to notify about crisis in session {SessionId}", sessionId);
                    return;
                }

                var notification = MindoraNotifications.AICrisisAlert(userName, sessionId);

                await _dispatcher.DispatchToManyAsync(adminIds, notification);

                _logger.LogWarning("Crisis alert dispatched to {Count} admins for session {SessionId}",
                    adminIds.Count, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch crisis alert for session {SessionId}", sessionId);
            }
        }

        private async Task<List<Guid>> GetAdminUserIdsAsync()
        {
            var adminRole = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null) return new List<Guid>();

            var adminUserIds = await _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.RoleId == adminRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            return adminUserIds;
        }

        // ============================
        // PRIVATE HELPER METHODS
        // ============================

        private string AnalyzeSentiment(string text)
        {
            var lower = text.ToLower();
            if (lower.Contains("sad") || lower.Contains("anxious") || lower.Contains("angry") || lower.Contains("depressed"))
                return "Negative";
            if (lower.Contains("happy") || lower.Contains("great") || lower.Contains("good") || lower.Contains("calm"))
                return "Positive";
            return "Neutral";
        }

        private double? EstimateMood(string text)
        {
            var sentiment = AnalyzeSentiment(text);
            return sentiment switch
            {
                "Positive" => 7.5,
                "Negative" => 3.5,
                _ => 5.0
            };
        }

        private string AnalyzeRisk(string text)
        {
            var lower = text.ToLower();
            if (lower.Contains("suicide") || lower.Contains("hurt myself") || lower.Contains("end my life"))
                return "High";
            if (lower.Contains("hopeless") || lower.Contains("can't cope") || lower.Contains("very anxious"))
                return "Moderate";
            return "Low";
        }

        private string DetectIntent(string text)
        {
            var lower = text.ToLower();
            if (lower.Contains("habit") || lower.Contains("addiction") || lower.Contains("relapse"))
                return "HabitHelp";
            if (lower.Contains("anxious") || lower.Contains("depressed") || lower.Contains("stress"))
                return "MentalWellness";
            if (lower.Contains("bored"))
                return "Relaxation";
            return "General";
        }

        private async Task<string> GenerateAIResponseAsync(string userMessage, string? lastContext)
        {
            var prompt = $@"
You are Mindora AI Wellness Coach. You are empathetic, supportive, and non-judgmental.
You help users with:
- Habit recovery
- Mental wellness
- Stress, anxiety, sadness
- Motivation and mindfulness
Never give medical diagnosis. If the user mentions self-harm or suicide, encourage them to seek immediate help and provide crisis resources.
User said: ""{userMessage}""
Please respond helpfully and compassionately.";

            try
            {
                return await _geminiService.GenerateResponseAsync(prompt, lastContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API call failed. Falling back to local response.");

                if (userMessage.ToLower().Contains("sad"))
                    return "I'm really sorry you're feeling this way. It's okay to feel sad. Would you like to talk about what's going on?";

                if (userMessage.ToLower().Contains("anxious"))
                    return "Anxiety can feel overwhelming. Try taking a few deep breaths. You're not alone—I'm here with you.";

                if (userMessage.ToLower().Contains("habit"))
                    return "Building a new habit takes time. What's one small step you can take today?";

                return "Thank you for sharing that with me. I'm here to support you. What would you like to focus on?";
            }
        }

        private AIWellnessCoachSessionDto MapSessionToDto(AIWellnessCoachSession session)
        {
            return new AIWellnessCoachSessionDto
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                UserName = session.User?.UserName ?? session.User?.Email ?? "User",
                StartedAt = session.StartedAt,
                EndedAt = session.EndedAt,
                IsActive = session.IsActive,
                LastContext = session.LastContext,
                MessageCount = session.Messages?.Count ?? 0,
                Messages = session.Messages?.Select(m => new AIChatMessageDto
                {
                    MessageId = m.MessageId,
                    SessionId = m.SessionId,
                    Sender = m.Sender,
                    Content = m.Content,
                    Sentiment = m.Sentiment,
                    MoodScore = m.MoodScore,
                    RiskLevel = m.RiskLevel,
                    Intent = m.Intent,
                    SentAt = m.SentAt
                }).ToList() ?? new List<AIChatMessageDto>()
            };
        }

        private AIChatMessageDto MapMessageToDto(AIChatMessage message)
        {
            return new AIChatMessageDto
            {
                MessageId = message.MessageId,
                SessionId = message.SessionId,
                Sender = message.Sender,
                Content = message.Content,
                Sentiment = message.Sentiment,
                MoodScore = message.MoodScore,
                RiskLevel = message.RiskLevel,
                Intent = message.Intent,
                SentAt = message.SentAt
            };
        }

        private string DetermineCategory(double score)
        {
            if (score >= 75) return "Excellent";
            if (score >= 50) return "Good";
            if (score >= 25) return "Fair";
            return "Poor";
        }
    }
}