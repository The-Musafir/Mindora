using System;

namespace Mindora.Domain.Entities
{
    public class Payout
    {
        public Guid PayoutId { get; set; }
        public Guid ProviderId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Bank"; // Bank, bKash, Nagad
        public string AccountDetails { get; set; } = string.Empty; // Bank account / wallet
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Paid
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedBy { get; set; }

        // Navigation
        public virtual ProfessionalProvider Provider { get; set; } = null!;
    }
}