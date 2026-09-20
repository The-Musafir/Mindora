using Mindora.Domain.Enums;

namespace Mindora.Application.DTOs.Habit
{
    public class CreateHabitRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public HabitFrequency Frequency { get; set; } = HabitFrequency.Daily;
        public HabitPriority Priority { get; set; } = HabitPriority.Medium;
        public HabitDifficulty Difficulty { get; set; } = HabitDifficulty.Medium;
        public string? Color { get; set; }
        public string? Icon { get; set; }
    }
}