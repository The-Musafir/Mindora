namespace Mindora.Domain.Entities
{
    public class HabitCategory
    {
        public Guid HabitCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public virtual ICollection<Habit> Habits { get; set; } = new List<Habit>();
    }
}