namespace Mindora.Application.DTOs.Journal
{
    public class JournalStatisticsViewModel
    {
        // ============================================================
        // OVERVIEW STATS
        // ============================================================
        public int TotalEntries { get; set; }
        public int EntriesThisMonth { get; set; }
        public double AverageMood { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public string? MostCommonMood { get; set; }

        // ============================================================
        // CHART DATA
        // ============================================================
        public List<MoodTimelinePoint> MoodTimeline { get; set; } = new();
        public List<MoodDistributionItem> MoodDistribution { get; set; } = new();
        public List<WritingHeatmapDay> HeatmapDays { get; set; } = new();

        // ============================================================
        // COMPUTED
        // ============================================================
        public string AverageMoodDisplay => AverageMood > 0 ? $"{AverageMood:F1}/10" : "—";
    }

    public class MoodTimelinePoint
    {
        public DateTime Date { get; set; }
        public int? MoodScore { get; set; }
    }

    public class MoodDistributionItem
    {
        public string MoodName { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Color { get; set; } = "#06B6D4";
    }

    public class WritingHeatmapDay
    {
        public DateTime Date { get; set; }
        public bool Wrote { get; set; }
    }
}