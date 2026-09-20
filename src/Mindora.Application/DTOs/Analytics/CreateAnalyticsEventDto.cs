namespace Mindora.Application.DTOs.Analytics
{
    public class CreateAnalyticsEventDto
    {
        public Guid? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Payload { get; set; }
    }
}