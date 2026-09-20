namespace Mindora.Application.DTOs.Habit
{
    public class HabitGoalDto
    {
        public Guid GoalId { get; set; }
        public decimal TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}