using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    /// <summary>
    /// An AI wellness coaching session.
    /// Contains all messages exchanged with the AI within this session.
    /// </summary>
    public class AIWellnessCoachSession
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public string? LastContext { get; set; }

        /// <summary>
        /// Optional user-defined title for this session.
        /// If null, title is derived from the first user message.
        /// </summary>
        public string? CustomTitle { get; set; }

        // ============================================================
        // Navigation Properties
        // ============================================================
        public virtual User User { get; set; } = null!;

        public virtual ICollection<AIChatMessage> Messages { get; set; } = new List<AIChatMessage>();

        public virtual ICollection<AIFeedback> Feedbacks { get; set; } = new List<AIFeedback>();

        public virtual ICollection<AIInteractionLog> InteractionLogs { get; set; } = new List<AIInteractionLog>();
    }
}