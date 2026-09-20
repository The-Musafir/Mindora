using System;

namespace Mindora.Domain.Entities
{
    public class AIFeedback
    {
        public Guid FeedbackId { get; set; }
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid? MessageId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual AIWellnessCoachSession Session { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual AIChatMessage? Message { get; set; }
    }
}