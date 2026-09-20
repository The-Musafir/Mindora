using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class CommunityPost
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public Guid? GroupId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Category { get; set; }
        public bool IsAnonymous { get; set; } = false;
        public bool IsPinned { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual CommunityGroup? Group { get; set; }
        public virtual ICollection<CommunityComment> Comments { get; set; } = new List<CommunityComment>();
        public virtual ICollection<CommunityReaction> Reactions { get; set; } = new List<CommunityReaction>();
    }
}