namespace Mindora.Application.DTOs.Professional
{
    public class AvailabilitySlotDto
    {
        public Guid SlotId { get; set; }
        public int DayOfWeek { get; set; } // 0=Sunday, 6=Saturday
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly? SpecificDate { get; set; }
    }
}