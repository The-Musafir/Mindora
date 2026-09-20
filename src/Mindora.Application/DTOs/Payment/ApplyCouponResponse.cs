namespace Mindora.Application.DTOs.Payment
{
    public class ApplyCouponResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}