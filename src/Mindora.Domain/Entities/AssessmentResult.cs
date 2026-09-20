namespace Mindora.Domain.Entities
{
    public class AssessmentResult
    {
        public Guid ResultId { get; set; }
        public Guid UserAssessmentId { get; set; }
        public int TotalScore { get; set; }
        public string SeverityLevel { get; set; } = string.Empty; // Low, Moderate, High
        public string? Interpretation { get; set; }

        public virtual UserAssessment UserAssessment { get; set; } = null!;
    }
}