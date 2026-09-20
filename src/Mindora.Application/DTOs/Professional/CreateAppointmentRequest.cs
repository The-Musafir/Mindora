namespace Mindora.Application.DTOs.Professional
{
    public class CreateAppointmentRequest
    {
        public Guid SlotId { get; set; }
        public Guid? ServiceId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
    }
}