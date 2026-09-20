using System;

namespace Mindora.Domain.Entities
{
    public class ConsultationPrescription
    {
        public Guid PrescriptionId { get; set; }
        public Guid SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ConsultationSession Session { get; set; } = null!;
    }
}