namespace Mindora.Application.DTOs.Professional
{
    public class CreateAvailabilitySlotRequest
    {
        public Guid PracticeId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRecurring { get; set; } = true;
        public DateOnly? SpecificDate { get; set; }
    }
}