namespace Mindora.Application.DTOs.Notification
{
    public class BroadcastNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Channel { get; set; } = "InApp"; // InApp, Email, Push
    }
}