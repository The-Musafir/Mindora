using System;

namespace Mindora.Domain.Entities
{
    public class ModerationAction
    {
        public Guid ActionId { get; set; }
        public Guid FlagId { get; set; }
        public Guid AdminUserId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime ActionedAt { get; set; } = DateTime.UtcNow;
        public virtual CommunityModerationFlag Flag { get; set; } = null!;
        public virtual User AdminUser { get; set; } = null!;
    }
}
