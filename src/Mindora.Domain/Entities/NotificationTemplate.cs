using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class NotificationTemplate
    {
        public Guid TemplateId { get; set; }
        public string TemplateKey { get; set; } = string.Empty;   // e.g., "HabitReminder", "AppointmentBooked"
        public string Subject { get; set; } = string.Empty;
        public string BodyTemplate { get; set; } = string.Empty;  // placeholders: {UserName}, {HabitName}
        public string Channel { get; set; } = "InApp";  // InApp, Email, Push
        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}