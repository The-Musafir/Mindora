namespace Mindora.Application.DTOs.Consultation
{
    public class ConsultationSessionDto
    {
        public Guid SessionId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string SessionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ConsultationNoteDto> NotesCollection { get; set; } = new();
        public List<ConsultationPrescriptionDto> Prescriptions { get; set; } = new();
        public List<ConsultationFeedbackDto> Feedbacks { get; set; } = new();
        public List<FollowUpPlanDto> FollowUpPlans { get; set; } = new();
    }
}