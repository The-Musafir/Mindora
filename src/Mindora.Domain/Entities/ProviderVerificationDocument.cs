using System;

namespace Mindora.Domain.Entities
{
    public class ProviderVerificationDocument
    {
        public Guid DocumentId { get; set; }
        public Guid ProviderId { get; set; }
        public string DocumentType { get; set; } = string.Empty; // License, Certification
        public string FileUrl { get; set; } = string.Empty;
        public DateTime? VerifiedAt { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        // Navigation Properties
        public virtual ProfessionalProvider Provider { get; set; } = null!;
    }
}