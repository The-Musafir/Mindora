namespace Mindora.Application.DTOs.Consultation
{
    public class ConsultationReminderDto
    {
        public Guid ReminderId { get; set; }
        public Guid SessionId { get; set; }
        public DateTime ReminderAt { get; set; }
        public string Channel { get; set; } = "InApp";
        public bool IsSent { get; set; }
    }
}