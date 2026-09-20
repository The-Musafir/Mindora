namespace Mindora.Application.DTOs.Habit
{
    public class HabitLogDto
    {
        public Guid HabitLogId { get; set; }
        public DateTime LogDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Note { get; set; }
        public string? Reflection { get; set; }
        public int? MoodBefore { get; set; }
        public int? MoodAfter { get; set; }
        public int? CompletionTimeMinutes { get; set; }
    }
}