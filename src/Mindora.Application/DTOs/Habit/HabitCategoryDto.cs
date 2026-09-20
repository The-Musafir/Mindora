namespace Mindora.Application.DTOs.Habit
{
    public class HabitCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}