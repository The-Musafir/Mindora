using System;

namespace Mindora.Domain.Entities
{
    public class UserNotificationPreference
    {
        public Guid PreferenceId { get; set; }
        public Guid UserId { get; set; }
        public bool EmailEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = true;
        public bool InAppEnabled { get; set; } = true;
        public bool QuietHoursEnabled { get; set; } = false;
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
        public string Frequency { get; set; } = "RealTime"; // RealTime, DailyDigest, WeeklySummary

        // Navigation
        public virtual User User { get; set; } = null!;
    }
}