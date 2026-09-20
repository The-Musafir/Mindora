using System;

namespace Mindora.Domain.Entities
{
    public class HabitGoal
    {
        public Guid GoalId { get; set; }
        public Guid HabitId { get; set; }
        public decimal TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public virtual Habit Habit { get; set; } = null!;
    }
}
