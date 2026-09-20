namespace Mindora.Application.DTOs.Assessment
{
    public class AssessmentStatisticsDto
    {
        public int TotalAssessments { get; set; }
        public int CompletedAssessments { get; set; }
        public int InProgressAssessments { get; set; }
        public int LowRiskCount { get; set; }
        public int ModerateRiskCount { get; set; }
        public int HighRiskCount { get; set; }
        public double AverageScore { get; set; }
    }
}