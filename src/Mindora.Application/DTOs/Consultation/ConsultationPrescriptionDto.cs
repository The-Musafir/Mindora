namespace Mindora.Application.DTOs.Consultation
{
    public class ConsultationPrescriptionDto
    {
        public Guid PrescriptionId { get; set; }
        public Guid SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}