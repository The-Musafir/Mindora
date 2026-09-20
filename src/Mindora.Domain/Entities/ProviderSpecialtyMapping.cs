using System;

namespace Mindora.Domain.Entities
{
    public class ProviderSpecialtyMapping
    {
        public Guid ProviderId { get; set; }
        public Guid SpecialtyId { get; set; }

        // Navigation Properties
        public virtual ProfessionalProvider Provider { get; set; } = null!;
        public virtual ProviderSpecialty Specialty { get; set; } = null!;
    }
}