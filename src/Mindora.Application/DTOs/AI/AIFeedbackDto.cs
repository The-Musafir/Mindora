namespace Mindora.Application.DTOs.AI
{
    public class AIFeedbackDto
    {
        public Guid FeedbackId { get; set; }
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid? MessageId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}