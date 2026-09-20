using System;

namespace Mindora.Domain.Entities
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid PaymentId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Payment Payment { get; set; } = null!;
    }
}