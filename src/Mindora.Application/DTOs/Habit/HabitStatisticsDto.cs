namespace Mindora.Application.DTOs.Habit
{
    public class HabitStatisticsDto
    {
        public int TotalLogs { get; set; }
        public int CompletedLogs { get; set; }
        public double CompletionRate { get; set; } // 0-100
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public double WeeklyAverage { get; set; }
        public double MonthlyAverage { get; set; }
        public int TotalRelapses { get; set; }
    }
}