using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAdminContentManagementService
    {
        // Wellness Resources
        Task<IReadOnlyList<AdminWellnessResourceDto>> GetAllWellnessResourcesAsync();
        Task<Guid> CreateWellnessResourceAsync(AdminWellnessResourceDto dto);
        Task<bool> ToggleWellnessResourceAsync(Guid resourceId, bool isPublished);

        // Assessment Questionnaires
        Task<IReadOnlyList<AdminAssessmentQuestionnaireDto>> GetAllAssessmentsAsync();
        Task<Guid> CreateAssessmentAsync(AdminAssessmentQuestionnaireDto dto);
        Task<bool> ToggleAssessmentAsync(Guid questionnaireId, bool isActive);
    }
}