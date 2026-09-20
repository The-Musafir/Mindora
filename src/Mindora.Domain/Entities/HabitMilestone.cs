using System;

namespace Mindora.Domain.Entities
{
    public class HabitMilestone
    {
        public Guid MilestoneId { get; set; }
        public Guid HabitId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime AchievedAt { get; set; }
        public virtual Habit Habit { get; set; } = null!;
    }
}
