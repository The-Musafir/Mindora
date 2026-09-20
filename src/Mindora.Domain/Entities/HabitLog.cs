namespace Mindora.Domain.Entities
{
    public class HabitLog
    {
        public Guid HabitLogId { get; set; }
        public Guid HabitId { get; set; }
        public DateTime LogDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Note { get; set; }
        public string? Reflection { get; set; }
        public int? MoodBefore { get; set; } // 1-10
        public int? MoodAfter { get; set; }  // 1-10
        public int? CompletionTimeMinutes { get; set; } // Future AI

        public virtual Habit Habit { get; set; } = null!;
    }
}