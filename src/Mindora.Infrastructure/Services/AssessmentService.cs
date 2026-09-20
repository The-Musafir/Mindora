using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Assessment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly MindoraDbContext _context;

        public AssessmentService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AssessmentQuestionnaireDto>> GetAllQuestionnairesAsync()
        {
            var questionnaires = await _context.AssessmentQuestionnaires
                .Where(q => q.IsActive)
                .Include(q => q.Questions)
                    .ThenInclude(question => question.Options)
                .ToListAsync();

            return questionnaires.Select(MapQuestionnaireToDto).ToList();
        }

        public async Task<AssessmentQuestionnaireDto?> GetQuestionnaireByIdAsync(Guid questionnaireId)
        {
            var questionnaire = await _context.AssessmentQuestionnaires
                .Where(q => q.QuestionnaireId == questionnaireId && q.IsActive)
                .Include(q => q.Questions)
                    .ThenInclude(question => question.Options)
                .FirstOrDefaultAsync();

            return questionnaire == null ? null : MapQuestionnaireToDto(questionnaire);
        }

        public async Task<AssessmentResultDto> SubmitAssessmentAsync(Guid userId, TakeAssessmentRequest request)
        {
            var questionnaire = await _context.AssessmentQuestionnaires
                .Include(q => q.Questions)
                    .ThenInclude(question => question.Options)
                .FirstOrDefaultAsync(q => q.QuestionnaireId == request.QuestionnaireId);

            if (questionnaire == null)
                throw new KeyNotFoundException("Questionnaire not found.");

            var userAssessment = new UserAssessment
            {
                UserAssessmentId = Guid.NewGuid(),
                UserId = userId,
                QuestionnaireId = request.QuestionnaireId,
                StartedAt = DateTime.UtcNow,
                Status = "InProgress"
            };

            _context.UserAssessments.Add(userAssessment);

            int totalScore = 0;
            int maxPossibleScore = 0;

            foreach (var question in questionnaire.Questions)
            {
                var answer = request.Answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                if (answer == null) continue;

                var userAnswer = new UserAssessmentAnswer
                {
                    AnswerId = Guid.NewGuid(),
                    UserAssessmentId = userAssessment.UserAssessmentId,
                    QuestionId = question.QuestionId,
                    SelectedOptionId = answer.SelectedOptionId,
                    FreeText = answer.FreeText
                };

                _context.UserAssessmentAnswers.Add(userAnswer);

                if (answer.SelectedOptionId.HasValue)
                {
                    var option = question.Options.FirstOrDefault(o => o.OptionId == answer.SelectedOptionId.Value);
                    if (option != null)
                    {
                        totalScore += option.ScoreValue;
                    }
                }

                maxPossibleScore += question.Options.Any() ? question.Options.Max(o => o.ScoreValue) : 0;
            }

            userAssessment.CompletedAt = DateTime.UtcNow;
            userAssessment.Status = "Completed";

            string severityLevel;
            string interpretation;

            double percentage = maxPossibleScore > 0 ? (double)totalScore / maxPossibleScore * 100 : 0;

            if (percentage <= 40)
            {
                severityLevel = "Low";
                interpretation = "Your responses suggest low levels of distress. Continue maintaining healthy habits.";
            }
            else if (percentage <= 70)
            {
                severityLevel = "Moderate";
                interpretation = "You may be experiencing moderate distress. Consider speaking with a professional for guidance.";
            }
            else
            {
                severityLevel = "High";
                interpretation = "Your responses indicate significant distress. We strongly recommend reaching out to a mental health professional.";
            }

            var result = new AssessmentResult
            {
                ResultId = Guid.NewGuid(),
                UserAssessmentId = userAssessment.UserAssessmentId,
                TotalScore = totalScore,
                SeverityLevel = severityLevel,
                Interpretation = interpretation
            };

            _context.AssessmentResults.Add(result);
            await _context.SaveChangesAsync();

            return new AssessmentResultDto
            {
                UserAssessmentId = userAssessment.UserAssessmentId,
                TotalScore = totalScore,
                SeverityLevel = severityLevel,
                Interpretation = interpretation
            };
        }

        public async Task<IReadOnlyList<UserAssessmentHistoryDto>> GetUserAssessmentHistoryAsync(Guid userId)
        {
            var assessments = await _context.UserAssessments
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Questionnaire)
                .Include(ua => ua.Result)
                .OrderByDescending(ua => ua.StartedAt)
                .ToListAsync();

            return assessments.Select(ua => new UserAssessmentHistoryDto
            {
                UserAssessmentId = ua.UserAssessmentId,
                QuestionnaireId = ua.QuestionnaireId,
                QuestionnaireTitle = ua.Questionnaire.Title,
                StartedAt = ua.StartedAt,
                CompletedAt = ua.CompletedAt,
                Status = ua.Status,
                TotalScore = ua.Result?.TotalScore,
                SeverityLevel = ua.Result?.SeverityLevel,
                Interpretation = ua.Result?.Interpretation
            }).ToList();
        }

        public async Task<AssessmentStatisticsDto> GetAssessmentStatisticsAsync(Guid userId)
        {
            var assessments = await _context.UserAssessments
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Result)
                .ToListAsync();

            var completed = assessments.Where(a => a.Status == "Completed").ToList();

            return new AssessmentStatisticsDto
            {
                TotalAssessments = assessments.Count,
                CompletedAssessments = completed.Count,
                InProgressAssessments = assessments.Count(a => a.Status == "InProgress"),
                LowRiskCount = completed.Count(a => a.Result?.SeverityLevel == "Low"),
                ModerateRiskCount = completed.Count(a => a.Result?.SeverityLevel == "Moderate"),
                HighRiskCount = completed.Count(a => a.Result?.SeverityLevel == "High"),
                AverageScore = completed.Any(a => a.Result != null)
                    ? completed.Where(a => a.Result != null).Average(a => a.Result!.TotalScore)
                    : 0
            };
        }

        private AssessmentQuestionnaireDto MapQuestionnaireToDto(AssessmentQuestionnaire questionnaire)
        {
            return new AssessmentQuestionnaireDto
            {
                QuestionnaireId = questionnaire.QuestionnaireId,
                Title = questionnaire.Title,
                Description = questionnaire.Description,
                IsActive = questionnaire.IsActive,
                Questions = questionnaire.Questions
                    .OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDto
                    {
                        QuestionId = q.QuestionId,
                        QuestionText = q.QuestionText,
                        OrderIndex = q.OrderIndex,
                        Options = q.Options != null
                            ? q.Options.Select(o => new OptionDto
                            {
                                OptionId = o.OptionId,
                                OptionText = o.OptionText,
                                ScoreValue = o.ScoreValue
                            }).ToList()
                            : new List<OptionDto>()
                    }).ToList()
            };
        }
    }
}