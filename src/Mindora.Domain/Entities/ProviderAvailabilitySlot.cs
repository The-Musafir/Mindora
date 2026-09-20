using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class ProviderAvailabilitySlot
    {
        public Guid SlotId { get; set; }
        public Guid PracticeId { get; set; }
        public int DayOfWeek { get; set; } // 0=Sunday, 6=Saturday
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRecurring { get; set; } = true;
        public DateOnly? SpecificDate { get; set; }

        // Navigation Properties
        public virtual ProviderPracticeDetail Practice { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}