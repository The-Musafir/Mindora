namespace Mindora.Application.DTOs.Notification
{
    public class UpdateNotificationPreferenceRequest
    {
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public bool InAppEnabled { get; set; }
        public bool QuietHoursEnabled { get; set; }
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
        public string Frequency { get; set; } = "RealTime";
    }
}