namespace Mindora.Application.DTOs.Notification
{
    public class NotificationPreferenceDto
    {
        public Guid PreferenceId { get; set; }
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public bool InAppEnabled { get; set; }
        public bool QuietHoursEnabled { get; set; }
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
        public string Frequency { get; set; } = "RealTime";
    }
}