namespace Mindora.Application.DTOs.Consultation
{
    public class UpdateConsultationSessionStatusRequest
    {
        public Guid SessionId { get; set; }
        public string Status { get; set; } = string.Empty; // Scheduled, InProgress, Completed, Cancelled, NoShow
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}