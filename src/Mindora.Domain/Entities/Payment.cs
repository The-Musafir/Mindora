using System;

namespace Mindora.Domain.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
        public string Gateway { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string PaymentType { get; set; } = "Subscription";
        public Guid? SubscriptionId { get; set; }
        public Guid? ConsultationPaymentId { get; set; }
        public string? IdempotencyKey { get; set; }
        public string? PaymentMethod { get; set; } // ✅ bKash, Nagad, Rocket, Card
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual UserSubscription? Subscription { get; set; }
        public virtual ConsultationPayment? ConsultationPayment { get; set; }
    }
}