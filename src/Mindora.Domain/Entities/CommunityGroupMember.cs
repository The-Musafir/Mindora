using System;

namespace Mindora.Domain.Entities
{
    public class CommunityGroupMember
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual CommunityGroup Group { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}