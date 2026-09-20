namespace Mindora.Application.DTOs.Consultation
{
    public class CreateConsultationPrescriptionRequest
    {
        public Guid SessionId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}