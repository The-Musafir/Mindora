using Mindora.Domain.Enums;

namespace Mindora.Application.DTOs.Habit
{
    public class HabitResponse
    {
        public Guid HabitId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public HabitFrequency Frequency { get; set; }
        public HabitPriority Priority { get; set; }
        public HabitDifficulty Difficulty { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Analytics/Streak
        public int CurrentStreak { get; set; }
        public double CompletionRate { get; set; }

        public List<HabitGoalDto> Goals { get; set; } = new();
        public List<HabitLogDto> Logs { get; set; } = new();
        public List<HabitReminderDto> Reminders { get; set; } = new();
        public List<RecoveryStreakDto> Streaks { get; set; } = new();
        public List<RelapseLogDto> Relapses { get; set; } = new();
        public List<HabitMilestoneDto> Milestones { get; set; } = new();
    }
}