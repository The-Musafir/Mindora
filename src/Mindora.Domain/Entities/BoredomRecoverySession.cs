using System;

namespace Mindora.Domain.Entities
{
    public class BoredomRecoverySession
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid ActivityId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? MoodBefore { get; set; }
        public int? MoodAfter { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual BoredomRecoveryActivity Activity { get; set; } = null!;
    }
}
