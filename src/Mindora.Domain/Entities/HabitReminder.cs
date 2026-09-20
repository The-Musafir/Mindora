namespace Mindora.Domain.Entities
{
    public class HabitReminder
    {
        public Guid HabitReminderId { get; set; }
        public Guid HabitId { get; set; }
        public TimeSpan ReminderTime { get; set; }
        public bool IsEnabled { get; set; } = true;

        public virtual Habit Habit { get; set; } = null!;
    }
}