using System;

namespace Mindora.Domain.Entities
{
    public class Coupon
    {
        public Guid CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "Percentage"; // Percentage, FixedAmount
        public decimal DiscountValue { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}