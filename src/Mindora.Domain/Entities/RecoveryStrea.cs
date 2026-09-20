namespace Mindora.Domain.Entities
{
    public class RecoveryStreak
    {
        public Guid RecoveryStreakId { get; set; }
        public Guid HabitId { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Habit Habit { get; set; } = null!;
    }
}