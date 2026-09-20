using System;

namespace Mindora.Domain.Entities
{
    public class RefundRequest
    {
        public Guid RefundId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedBy { get; set; }

        // Navigation
        public virtual Payment Payment { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}