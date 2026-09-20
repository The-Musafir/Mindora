namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminAssessmentQuestionnaireDto
    {
        public Guid QuestionnaireId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}