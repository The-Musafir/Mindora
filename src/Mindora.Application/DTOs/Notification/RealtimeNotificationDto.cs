namespace Mindora.Application.DTOs.Notification
{
    
    public class RealtimeNotificationDto
    {
        // ================================================================
        // Existing UserNotification entity এর সাথে matching fields
        // ================================================================
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public Guid? TemplateId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        /// <summary>InApp, Email, Push</summary>
        public string Channel { get; set; } = "InApp";

        /// <summary>HabitReminder, JournalReminder, Community, Payment, General etc.</summary>
        public string Type { get; set; } = "General";

        /// <summary>সম্পর্কিত entity এর Id — HabitId, PostId, AppointmentId etc.</summary>
        public Guid? ReferenceId { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }

        // ================================================================
        // Realtime UI-specific fields (নতুন)
        // ================================================================

       
        public string Severity { get; set; } = "Info";

        
        public string? ActionUrl { get; set; }

     
        public string? Icon { get; set; }

        
        public int? UnreadCount { get; set; }

        
        public bool IsSoundEnabled { get; set; } = true;
    }
}