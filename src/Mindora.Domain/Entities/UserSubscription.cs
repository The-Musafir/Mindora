using System;

namespace Mindora.Domain.Entities
{
    public class UserSubscription
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual SubscriptionPlan Plan { get; set; } = null!;
    }
}