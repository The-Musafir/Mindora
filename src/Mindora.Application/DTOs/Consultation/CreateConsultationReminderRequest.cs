namespace Mindora.Application.DTOs.Consultation
{
    public class CreateConsultationReminderRequest
    {
        public Guid SessionId { get; set; }
        public DateTime ReminderAt { get; set; }
        public string Channel { get; set; } = "InApp";
    }
}