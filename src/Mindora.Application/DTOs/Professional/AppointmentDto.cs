namespace Mindora.Application.DTOs.Professional
{
    public class AppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public Guid SlotId { get; set; }
        public Guid UserId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AppointmentStatusHistoryDto> StatusHistory { get; set; } = new();
    }
}