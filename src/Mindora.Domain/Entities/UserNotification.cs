using System;

namespace Mindora.Domain.Entities
{
    public class UserNotification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public Guid? TemplateId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Channel { get; set; } = "InApp";  // InApp, Email, Push
        public string Type { get; set; } = "General";   // HabitReminder, JournalReminder, Community, etc.
        public Guid? ReferenceId { get; set; }          // e.g., HabitId, PostId, AppointmentId
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual NotificationTemplate? Template { get; set; }
        public virtual NotificationLog? Log { get; set; }
    }
}