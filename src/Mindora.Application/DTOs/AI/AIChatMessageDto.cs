namespace Mindora.Application.DTOs.AI
{
    public class AIChatMessageDto
    {
        public Guid MessageId { get; set; }
        public Guid SessionId { get; set; }
        public string Sender { get; set; } = "User";
        public string Content { get; set; } = string.Empty;
        public string? Sentiment { get; set; }
        public double? MoodScore { get; set; }
        public string? RiskLevel { get; set; }
        public string? Intent { get; set; }
        public DateTime SentAt { get; set; }
    }
}