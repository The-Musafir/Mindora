using System;

namespace Mindora.Domain.Entities
{
    public class FollowUpPlan
    {
        public Guid FollowUpPlanId { get; set; }
        public Guid SessionId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;

        // Navigation
        public virtual ConsultationSession Session { get; set; } = null!;
    }
}