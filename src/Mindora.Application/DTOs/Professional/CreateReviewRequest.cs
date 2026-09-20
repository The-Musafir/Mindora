namespace Mindora.Application.DTOs.Professional
{
    public class CreateReviewRequest
    {
        public Guid ProviderId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
    }
}