using Mindora.Domain.Enums;

namespace Mindora.Domain.Entities
{
    public class Habit
    {
        public Guid HabitId { get; set; }
        public Guid UserId { get; set; }
        public Guid? HabitCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public HabitFrequency Frequency { get; set; } = HabitFrequency.Daily;
        public HabitPriority Priority { get; set; } = HabitPriority.Medium;
        public HabitDifficulty Difficulty { get; set; } = HabitDifficulty.Medium;

        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsArchived { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual HabitCategory? HabitCategory { get; set; }
        public virtual ICollection<HabitGoal> Goals { get; set; } = new List<HabitGoal>();
        public virtual ICollection<HabitLog> Logs { get; set; } = new List<HabitLog>();
        public virtual ICollection<HabitReminder> Reminders { get; set; } = new List<HabitReminder>();
        public virtual ICollection<RecoveryStreak> Streaks { get; set; } = new List<RecoveryStreak>();
        public virtual ICollection<RelapseLog> Relapses { get; set; } = new List<RelapseLog>();
        public virtual ICollection<HabitMilestone> Milestones { get; set; } = new List<HabitMilestone>();
    }
}