using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class ProviderPracticeDetail
    {
        public Guid PracticeId { get; set; }
        public Guid ProviderId { get; set; }
        public string PracticeName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsVirtual { get; set; } = false;

        // Navigation Properties
        public virtual ProfessionalProvider Provider { get; set; } = null!;
        public virtual ICollection<ProviderService> Services { get; set; } = new List<ProviderService>();
        public virtual ICollection<ProviderAvailabilitySlot> AvailabilitySlots { get; set; } = new List<ProviderAvailabilitySlot>();
    }
}