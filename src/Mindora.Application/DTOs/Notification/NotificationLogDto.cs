namespace Mindora.Application.DTOs.Notification
{
    public class NotificationLogDto
    {
        public Guid LogId { get; set; }
        public Guid NotificationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}