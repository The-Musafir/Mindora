using System;

namespace Mindora.Domain.Entities
{
    public class ConsultationFeedback
    {
        public Guid FeedbackId { get; set; }
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ConsultationSession Session { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}