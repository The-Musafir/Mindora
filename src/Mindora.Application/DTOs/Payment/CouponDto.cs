namespace Mindora.Application.DTOs.Payment
{
    public class CouponDto
    {
        public Guid CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }   
        public bool IsActive { get; set; }
    }
}