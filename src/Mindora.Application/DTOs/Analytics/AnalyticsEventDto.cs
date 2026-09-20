namespace Mindora.Application.DTOs.Analytics
{
    public class AnalyticsEventDto
    {
        public long EventId { get; set; }
        public Guid? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }
}