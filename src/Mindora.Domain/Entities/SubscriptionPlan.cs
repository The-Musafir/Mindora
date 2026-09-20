using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class SubscriptionPlan
    {
        public Guid PlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }          // মাসিক মূল্য (USD)
        public int DurationDays { get; set; }        // 30, 365
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}