namespace Mindora.Application.DTOs.Consultation
{
    public class CreateConsultationFeedbackRequest
    {
        public Guid SessionId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
    }
}