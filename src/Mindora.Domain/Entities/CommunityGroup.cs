using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class CommunityGroup
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<CommunityGroupMember> Members { get; set; } = new List<CommunityGroupMember>();
        public virtual ICollection<CommunityPost> Posts { get; set; } = new List<CommunityPost>();
    }
}