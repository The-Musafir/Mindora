namespace Mindora.Application.DTOs.AI
{
    public class CreateAIFeedbackRequest
    {
        public Guid SessionId { get; set; }
        public Guid? MessageId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
    }
}