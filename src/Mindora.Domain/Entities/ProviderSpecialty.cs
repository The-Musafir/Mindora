using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class ProviderSpecialty
    {
        public Guid SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<ProviderSpecialtyMapping> ProviderMappings { get; set; } = new List<ProviderSpecialtyMapping>();
    }
}