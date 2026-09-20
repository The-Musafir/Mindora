namespace Mindora.Application.DTOs.Assessment
{
    public class AssessmentStatisticsViewModel
    {
        // ============================================================
        // BASE STATS
        // ============================================================
        public int TotalAssessments { get; set; }
        public int CompletedAssessments { get; set; }
        public int InProgressAssessments { get; set; }
        public double AverageScore { get; set; }

        // ============================================================
        // RISK BREAKDOWN
        // ============================================================
        public int LowRiskCount { get; set; }
        public int ModerateRiskCount { get; set; }
        public int HighRiskCount { get; set; }

        // ============================================================
        // CHART DATA
        // ============================================================
        /// <summary>Score progression over time (line chart).</summary>
        public List<AssessmentScorePoint> ScoreHistory { get; set; } = new();

        /// <summary>Assessment types breakdown (doughnut).</summary>
        public List<AssessmentTypeCount> TypeBreakdown { get; set; } = new();

        // ============================================================
        // COMPUTED
        // ============================================================
        public string AverageScoreDisplay => AverageScore > 0 ? $"{AverageScore:F1}" : "—";
        public int TotalRisk => LowRiskCount + ModerateRiskCount + HighRiskCount;
        public bool HasData => TotalAssessments > 0;
    }

    public class AssessmentScorePoint
    {
        public DateTime Date { get; set; }
        public double Score { get; set; }
        public string QuestionnaireTitle { get; set; } = string.Empty;
    }

    public class AssessmentTypeCount
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}