using Mindora.Application.DTOs.Assessment;

namespace Mindora.Application.Interfaces
{
    public interface IAssessmentService
    {
        // Questionnaire
        Task<IReadOnlyList<AssessmentQuestionnaireDto>> GetAllQuestionnairesAsync();
        Task<AssessmentQuestionnaireDto?> GetQuestionnaireByIdAsync(Guid questionnaireId);

        // Take Assessment (submit answers)
        Task<AssessmentResultDto> SubmitAssessmentAsync(Guid userId, TakeAssessmentRequest request);

        // History
        Task<IReadOnlyList<UserAssessmentHistoryDto>> GetUserAssessmentHistoryAsync(Guid userId);

        // Statistics
        Task<AssessmentStatisticsDto> GetAssessmentStatisticsAsync(Guid userId);
    }
}