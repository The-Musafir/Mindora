namespace Mindora.Application.DTOs.Payment
{
    public class ConsultationPaymentDto
    {
        public Guid ConsultationPaymentId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid ProfessionalId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}