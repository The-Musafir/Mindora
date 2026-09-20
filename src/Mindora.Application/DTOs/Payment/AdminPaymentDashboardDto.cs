namespace Mindora.Application.DTOs.Payment
{
    public class AdminPaymentDashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalPayments { get; set; }
        public int PendingRefundsCount { get; set; }
        public int ActiveSubscriptionsCount { get; set; }
        public int PendingPayoutsCount { get; set; }
        public int ActiveCouponsCount { get; set; }

        public List<PaymentResponseDto> RecentPayments { get; set; } = new();
        public List<RefundRequestDto> PendingRefunds { get; set; } = new();
        public List<PayoutDto> PendingPayouts { get; set; } = new();
        public List<CouponDto> ActiveCoupons { get; set; } = new();
    }
}