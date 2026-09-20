using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class CommunityModerationFlag
    {
        public Guid FlagId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public Guid ReportedByUserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Reviewed, ActionTaken
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual CommunityPost? Post { get; set; }
        public virtual CommunityComment? Comment { get; set; }
        public virtual User ReportedByUser { get; set; } = null!;
        public virtual ICollection<ModerationAction> ModerationActions { get; set; } = new List<ModerationAction>();
    }
}