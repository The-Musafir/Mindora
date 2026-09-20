using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class ProfessionalProvider
    {
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ProviderSpecialtyMapping> SpecialtyMappings { get; set; } = new List<ProviderSpecialtyMapping>();
        public virtual ICollection<ProviderPracticeDetail> PracticeDetails { get; set; } = new List<ProviderPracticeDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<ProviderVerificationDocument> VerificationDocuments { get; set; } = new List<ProviderVerificationDocument>();
    }
}