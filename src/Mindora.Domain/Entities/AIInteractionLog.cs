using System;

namespace Mindora.Domain.Entities
{
    public class AIInteractionLog
    {
        public Guid LogId { get; set; }
        public Guid SessionId { get; set; }
        public Guid? TemplateId { get; set; }
        public string Action { get; set; } = string.Empty; // Sent, Received, CrisisTriggered
        public string? Metadata { get; set; }               // JSON payload
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual AIWellnessCoachSession Session { get; set; } = null!;
        public virtual AIPromptTemplate? Template { get; set; }
    }
}