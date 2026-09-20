namespace Mindora.Application.DTOs.Habit
{
    public class RecoveryStreakDto
    {
        public Guid RecoveryStreakId { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}