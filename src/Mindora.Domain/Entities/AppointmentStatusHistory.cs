using System;

namespace Mindora.Domain.Entities
{
    public class AppointmentStatusHistory
    {
        public Guid HistoryId { get; set; }
        public Guid AppointmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Appointment Appointment { get; set; } = null!;
    }
}