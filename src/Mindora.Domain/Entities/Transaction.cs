using System;

namespace Mindora.Domain.Entities
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty; // Success, Failed, Refunded
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual Payment Payment { get; set; } = null!;
    }
}