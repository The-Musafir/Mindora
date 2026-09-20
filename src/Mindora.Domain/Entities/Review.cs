using System;

namespace Mindora.Domain.Entities
{
    public class Review
    {
        public Guid ReviewId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ProfessionalProvider Provider { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}