namespace Mindora.Application.DTOs.Notification
{
    public class CreateNotificationRequest
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Channel { get; set; } = "InApp";
        public string Type { get; set; } = "General";
        public Guid? ReferenceId { get; set; }
        public Guid? TemplateId { get; set; }
    }
}