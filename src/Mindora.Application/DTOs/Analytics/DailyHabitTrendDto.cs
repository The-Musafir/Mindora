namespace Mindora.Application.DTOs.Analytics
{
    public class DailyHabitTrendDto
    {
        public DateTime Date { get; set; }
        public int CompletedHabits { get; set; }
        public int TotalHabits { get; set; }
        public double CompletionRate { get; set; }
    }
}