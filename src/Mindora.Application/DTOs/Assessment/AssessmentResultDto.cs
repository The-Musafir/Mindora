namespace Mindora.Application.DTOs.Assessment
{
    public class AssessmentResultDto
    {
        public Guid UserAssessmentId { get; set; }
        public int TotalScore { get; set; }
        public string SeverityLevel { get; set; } = string.Empty;
        public string? Interpretation { get; set; }
    }
}