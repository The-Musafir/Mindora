namespace Mindora.Application.DTOs.Consultation
{
    public class CreateConsultationSessionRequest
    {
        public Guid? AppointmentId { get; set; }
        public Guid ProviderId { get; set; }
        public string SessionType { get; set; } = "Chat";
        public DateTime ScheduledAt { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
    }
}