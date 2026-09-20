namespace Mindora.Application.DTOs.Analytics
{
    public class UserAnalyticsDto
    {
        // Habit Analytics
        public int TotalHabits { get; set; }
        public int ActiveHabits { get; set; }
        public int CompletedHabitLogs { get; set; }
        public int TotalHabitLogs { get; set; }
        public double HabitCompletionRate { get; set; }
        public int CurrentLongestStreak { get; set; }
        public int TotalRelapses { get; set; }

        // Journal Analytics
        public int TotalJournalEntries { get; set; }
        public int JournalEntriesThisMonth { get; set; }

        // Assessment Analytics
        public int TotalAssessmentsTaken { get; set; }
        public double AverageAssessmentScore { get; set; }
        public string? LatestRiskLevel { get; set; }

        // AI Analytics
        public int TotalAIConversations { get; set; }
        public int AIMessagesSent { get; set; }

        // Wellness
        public double LatestWellnessScore { get; set; }
        public string? LatestWellnessCategory { get; set; }

        // Mood
        public double AverageMoodScore { get; set; }

        // Trend Data (for charts)
        public List<DailyMoodTrendDto> MoodTrend { get; set; } = new();
        public List<DailyHabitTrendDto> HabitTrend { get; set; } = new();
    }
}