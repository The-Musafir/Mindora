namespace Mindora.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}