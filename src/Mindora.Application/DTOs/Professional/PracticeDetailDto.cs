namespace Mindora.Application.DTOs.Professional
{
    public class PracticeDetailDto
    {
        public Guid PracticeId { get; set; }
        public string PracticeName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsVirtual { get; set; }
        public List<ServiceDto> Services { get; set; } = new();
        public List<AvailabilitySlotDto> AvailabilitySlots { get; set; } = new();
    }
}