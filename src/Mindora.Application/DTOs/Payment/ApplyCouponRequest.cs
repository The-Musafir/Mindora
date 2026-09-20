namespace Mindora.Application.DTOs.Payment
{
    public class ApplyCouponRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
    }
}