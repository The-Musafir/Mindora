namespace Mindora.Application.DTOs.Professional
{
    public class AppointmentStatusHistoryDto
    {
        public Guid HistoryId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}