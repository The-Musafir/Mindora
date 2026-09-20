namespace Mindora.Application.DTOs.Consultation
{
    public class ConsultationFeedbackDto
    {
        public Guid FeedbackId { get; set; }
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}