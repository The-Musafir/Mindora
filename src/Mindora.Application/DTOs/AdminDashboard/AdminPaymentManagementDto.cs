using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminPaymentManagementDto
    {
        public List<PaymentResponseDto> Payments { get; set; } = new();
        public List<RefundRequestDto> Refunds { get; set; } = new();
        public List<PayoutDto> Payouts { get; set; } = new();
        public List<CouponDto> Coupons { get; set; } = new();
    }
}