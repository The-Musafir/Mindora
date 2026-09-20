using System;

namespace Mindora.Domain.Entities
{
    public class CommunityReaction
    {
        public Guid ReactionId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public Guid UserId { get; set; }
        public string ReactionType { get; set; } = string.Empty; // Like, Support, Insightful
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual CommunityPost? Post { get; set; }
        public virtual CommunityComment? Comment { get; set; }
        public virtual User User { get; set; } = null!;
    }
}