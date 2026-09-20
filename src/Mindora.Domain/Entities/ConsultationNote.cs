using System;

namespace Mindora.Domain.Entities
{
    public class ConsultationNote
    {
        public Guid NoteId { get; set; }
        public Guid SessionId { get; set; }
        public Guid ProviderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ConsultationSession Session { get; set; } = null!;
        public virtual ProfessionalProvider Provider { get; set; } = null!;
    }
}