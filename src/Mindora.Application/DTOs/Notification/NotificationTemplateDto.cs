namespace Mindora.Application.DTOs.Notification
{
    public class NotificationTemplateDto
    {
        public Guid TemplateId { get; set; }
        public string TemplateKey { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string BodyTemplate { get; set; } = string.Empty;
        public string Channel { get; set; } = "InApp";
        public bool IsActive { get; set; }
    }
}