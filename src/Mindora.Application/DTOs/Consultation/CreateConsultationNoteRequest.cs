namespace Mindora.Application.DTOs.Consultation
{
    public class CreateConsultationNoteRequest
    {
        public Guid SessionId { get; set; }
        public Guid ProviderId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}