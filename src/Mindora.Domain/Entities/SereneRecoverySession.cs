using System;

namespace Mindora.Domain.Entities
{
    public class SereneRecoverySession
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ResourceId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Feedback { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual SereneRecoveryResource? Resource { get; set; }
    }
}
