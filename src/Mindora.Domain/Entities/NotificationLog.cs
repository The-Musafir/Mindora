using System;

namespace Mindora.Domain.Entities
{
    public class NotificationLog
    {
        public Guid LogId { get; set; }
        public Guid NotificationId { get; set; }
        public string Status { get; set; } = "Sent";  // Sent, Delivered, Read, Failed
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual UserNotification Notification { get; set; } = null!;
    }
}