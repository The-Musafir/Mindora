using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;
using Mindora.Web.Hubs;

namespace Mindora.Web.Services
{
    /// <summary>
    /// Streams AI responses over SignalR.
    ///
    /// Flow:
    ///   1. Save user message to DB (AIChatMessage, Sender = "User")
    ///   2. Call Gemini to get full response
    ///   3. Split response into word chunks
    ///   4. Stream chunks via SignalR with small delay (typewriter effect)
    ///   5. Save AI message to DB (AIChatMessage, Sender = "AI")
    /// </summary>
    public class AIChatStreamingService : IAIChatStreamingService
    {
        private readonly MindoraDbContext _context;
        private readonly IGeminiService _geminiService;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<AIChatStreamingService> _logger;

        // Streaming tuning
        private const int CHUNK_SIZE = 3;         // words per chunk
        private const int CHUNK_DELAY_MS = 40;    // delay between chunks

        public AIChatStreamingService(
            MindoraDbContext context,
            IGeminiService geminiService,
            IHubContext<ChatHub> chatHub,
            ILogger<AIChatStreamingService> logger)
        {
            _context = context;
            _geminiService = geminiService;
            _chatHub = chatHub;
            _logger = logger;
        }

        public async Task StreamResponseAsync(
            Guid userId,
            Guid sessionId,
            string userMessage,
            string connectionId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            // 1. Validate session belongs to user
            var session = await _context.AIWellnessCoachSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId,
                    cancellationToken);

            if (session == null)
            {
                await SendErrorAsync(connectionId, "Session not found or access denied.");
                return;
            }

            try
            {
                // 2. Save user message
                var userMsg = new AIChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    SessionId = sessionId,
                    Sender = "User",
                    Content = userMessage,
                    SentAt = DateTime.UtcNow,
                    Sentiment = "Neutral",
                    RiskLevel = AnalyzeRisk(userMessage),
                    Intent = "General"
                };
                _context.AIChatMessages.Add(userMsg);
                await _context.SaveChangesAsync(cancellationToken);

                // Notify client that streaming has started
                await _chatHub.Clients.Client(connectionId)
                    .SendAsync("AIStreamStarted", new
                    {
                        userMessageId = userMsg.MessageId,
                        sessionId,
                        startedAt = DateTime.UtcNow
                    }, cancellationToken);

                // 3. Call Gemini for full response
                var fullResponse = await GenerateAIResponseAsync(
                    userMessage, session.LastContext, cancellationToken);

                if (string.IsNullOrWhiteSpace(fullResponse))
                {
                    fullResponse = "I'm here for you. Could you tell me more?";
                }

                // 4. Stream chunk by chunk
                var chunks = SplitIntoChunks(fullResponse, CHUNK_SIZE);
                var accumulated = new System.Text.StringBuilder();

                foreach (var chunk in chunks)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    accumulated.Append(chunk);
                    accumulated.Append(' ');

                    await _chatHub.Clients.Client(connectionId)
                        .SendAsync("AIStreamChunk", new
                        {
                            sessionId,
                            chunk,
                            accumulated = accumulated.ToString().TrimEnd()
                        }, cancellationToken);

                    // Small delay for typewriter effect
                    await Task.Delay(CHUNK_DELAY_MS, cancellationToken);
                }

                // 5. Save AI message
                var aiMsg = new AIChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    SessionId = sessionId,
                    Sender = "AI",
                    Content = fullResponse,
                    SentAt = DateTime.UtcNow,
                    Sentiment = "Neutral",
                    RiskLevel = AnalyzeRisk(userMessage),
                    Intent = "General"
                };
                _context.AIChatMessages.Add(aiMsg);
                session.LastContext = $"{userMessage} | {fullResponse}";
                await _context.SaveChangesAsync(cancellationToken);

                // Notify client that streaming is complete
                await _chatHub.Clients.Client(connectionId)
                    .SendAsync("AIStreamCompleted", new
                    {
                        sessionId,
                        aiMessageId = aiMsg.MessageId,
                        fullContent = fullResponse,
                        completedAt = DateTime.UtcNow
                    }, cancellationToken);

                _logger.LogInformation(
                    "AI streaming completed for user {UserId}, session {SessionId}",
                    userId, sessionId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("AI streaming cancelled by user");
                await SendErrorAsync(connectionId, "cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI streaming failed");
                await SendErrorAsync(connectionId, "AI service error. Please try again.");
            }
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================

        private async Task<string> GenerateAIResponseAsync(
            string userMessage,
            string? lastContext,
            CancellationToken cancellationToken)
        {
            var prompt = $@"
You are Mindora AI Wellness Coach. You are empathetic, supportive, and non-judgmental.
You help users with habit recovery, mental wellness, stress, anxiety, and motivation.
Never give medical diagnosis. If the user mentions self-harm or suicide, encourage them to seek immediate help.
Keep responses concise and warm (max 3 paragraphs).
User said: ""{userMessage}""
Respond helpfully.";

            try
            {
                return await _geminiService.GenerateResponseAsync(prompt, lastContext);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gemini call failed, using fallback");
                return GetFallbackResponse(userMessage);
            }
        }

        private static List<string> SplitIntoChunks(string text, int wordsPerChunk)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();

            for (int i = 0; i < words.Length; i += wordsPerChunk)
            {
                var count = Math.Min(wordsPerChunk, words.Length - i);
                chunks.Add(string.Join(' ', words.Skip(i).Take(count)));
            }

            return chunks;
        }

        private static string GetFallbackResponse(string userMessage)
        {
            var lower = userMessage.ToLower();
            if (lower.Contains("sad") || lower.Contains("depressed"))
                return "I'm really sorry you're feeling this way. It's okay to feel sad. Would you like to talk about what's going on?";
            if (lower.Contains("anxious") || lower.Contains("stress"))
                return "Anxiety can feel overwhelming. Take a few deep breaths. You're not alone—I'm here with you.";
            if (lower.Contains("habit") || lower.Contains("relapse"))
                return "Building a new habit takes time. What's one small step you can take today?";
            return "Thank you for sharing that with me. I'm here to support you. What would you like to focus on?";
        }

        private static string AnalyzeRisk(string text)
        {
            var lower = text.ToLower();
            if (lower.Contains("suicide") || lower.Contains("hurt myself") || lower.Contains("end my life"))
                return "High";
            if (lower.Contains("hopeless") || lower.Contains("can't cope"))
                return "Moderate";
            return "Low";
        }

        private async Task SendErrorAsync(string connectionId, string message)
        {
            try
            {
                await _chatHub.Clients.Client(connectionId)
                    .SendAsync("AIStreamError", new { error = message });
            }
            catch { /* silent */ }
        }
    }
}