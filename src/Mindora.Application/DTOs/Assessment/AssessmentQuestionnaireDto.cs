namespace Mindora.Application.DTOs.Assessment
{
    public class AssessmentQuestionnaireDto
    {
        public Guid QuestionnaireId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }
}