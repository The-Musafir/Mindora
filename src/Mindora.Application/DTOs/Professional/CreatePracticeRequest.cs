namespace Mindora.Application.DTOs.Professional
{
    public class CreatePracticeRequest
    {
        public Guid ProviderId { get; set; }
        public string PracticeName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsVirtual { get; set; }
    }
}