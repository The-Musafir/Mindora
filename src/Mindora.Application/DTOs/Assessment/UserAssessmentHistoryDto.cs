namespace Mindora.Application.DTOs.Assessment
{
    public class UserAssessmentHistoryDto
    {
        public Guid UserAssessmentId { get; set; }
        public Guid QuestionnaireId { get; set; }
        public string QuestionnaireTitle { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? TotalScore { get; set; }
        public string? SeverityLevel { get; set; }
        public string? Interpretation { get; set; }   
    }
}