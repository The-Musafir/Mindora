using System;

namespace Mindora.Domain.Entities
{
    public class AIPromptTemplate
    {
        public Guid TemplateId { get; set; }
        public string TemplateKey { get; set; } = string.Empty; // e.g., HabitCoach, CrisisSupport
        public string Title { get; set; } = string.Empty;
        public string PromptText { get; set; } = string.Empty;
        public string Tone { get; set; } = "Supportive"; // Supportive, Professional, Friendly
        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<AIInteractionLog> InteractionLogs { get; set; } = new List<AIInteractionLog>();
    }
}