using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class Appointment
    {
        public Guid AppointmentId { get; set; }
        public Guid SlotId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ServiceId { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, NoShow
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ProviderAvailabilitySlot Slot { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ProviderService? Service { get; set; }
        public virtual ICollection<AppointmentStatusHistory> StatusHistories { get; set; } = new List<AppointmentStatusHistory>();
    }
}