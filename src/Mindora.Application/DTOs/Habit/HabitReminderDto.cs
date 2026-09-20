namespace Mindora.Application.DTOs.Habit
{
    public class HabitReminderDto
    {
        public Guid HabitReminderId { get; set; }
        public TimeSpan ReminderTime { get; set; }
        public bool IsEnabled { get; set; }
    }
}