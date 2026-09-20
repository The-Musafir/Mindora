namespace Mindora.Application.DTOs.Habit
{
    public class HabitMilestoneDto
    {
        public Guid MilestoneId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime AchievedAt { get; set; }
    }
}