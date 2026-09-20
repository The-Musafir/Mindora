using System;

namespace Mindora.Domain.Entities
{
    public class BoxBreathingSession
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public int DurationSeconds { get; set; }
        public DateTime CompletedAt { get; set; }
        public string? HeartRateVariability { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
