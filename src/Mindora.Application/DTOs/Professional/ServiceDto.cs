namespace Mindora.Application.DTOs.Professional
{
    public class ServiceDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal? Fee { get; set; }
    }
}