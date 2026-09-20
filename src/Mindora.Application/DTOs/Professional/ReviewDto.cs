namespace Mindora.Application.DTOs.Professional
{
    public class ReviewDto
    {
        public Guid ReviewId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}