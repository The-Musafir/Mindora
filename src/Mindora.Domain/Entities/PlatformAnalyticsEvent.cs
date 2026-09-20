using System;

namespace Mindora.Domain.Entities
{
    public class PlatformAnalyticsEvent
    {
        public long EventId { get; set; }
        public Guid? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Payload { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}