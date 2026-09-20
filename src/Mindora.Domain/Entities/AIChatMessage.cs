using System;

namespace Mindora.Domain.Entities
{
    public class AIChatMessage
    {
        public Guid MessageId { get; set; }
        public Guid SessionId { get; set; }
        public string Sender { get; set; } = "User"; // User, AI
        public string Content { get; set; } = string.Empty;
        public string? Sentiment { get; set; }           // Positive, Negative, Neutral
        public double? MoodScore { get; set; }          // 1-10
        public string? RiskLevel { get; set; }          // Low, Moderate, High
        public string? Intent { get; set; }             // HabitHelp, Relaxation, General
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string? Metadata { get; set; }           // AI model info JSON

        // Navigation
        public virtual AIWellnessCoachSession Session { get; set; } = null!;
    }
}