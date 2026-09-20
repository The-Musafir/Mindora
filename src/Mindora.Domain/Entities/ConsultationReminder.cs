using System;

namespace Mindora.Domain.Entities
{
    public class ConsultationReminder
    {
        public Guid ReminderId { get; set; }
        public Guid SessionId { get; set; }
        public DateTime ReminderAt { get; set; }
        public string Channel { get; set; } = "InApp"; // InApp, Email, Push
        public bool IsSent { get; set; } = false;

        // Navigation
        public virtual ConsultationSession Session { get; set; } = null!;
    }
}