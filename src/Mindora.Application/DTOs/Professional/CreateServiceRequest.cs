namespace Mindora.Application.DTOs.Professional
{
    public class CreateServiceRequest
    {
        public Guid PracticeId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal? Fee { get; set; }
    }
}