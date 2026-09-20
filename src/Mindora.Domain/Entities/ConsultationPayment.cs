using System;

namespace Mindora.Domain.Entities
{
    public class ConsultationPayment
    {
        public Guid ConsultationPaymentId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid ProfessionalId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Completed, Refunded
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual ProfessionalProvider Professional { get; set; } = null!;
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual Payment? Payment { get; set; }
    }
}