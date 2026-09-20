using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class CommunityComment
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual CommunityPost Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual CommunityComment? ParentComment { get; set; }
        public virtual ICollection<CommunityComment> Replies { get; set; } = new List<CommunityComment>();
        public virtual ICollection<CommunityReaction> Reactions { get; set; } = new List<CommunityReaction>();
    }
}