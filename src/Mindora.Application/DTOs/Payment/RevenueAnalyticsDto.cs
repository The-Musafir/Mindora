namespace Mindora.Application.DTOs.Payment
{
    public class RevenueAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public decimal SubscriptionRevenue { get; set; }
        public decimal ConsultationRevenue { get; set; }
        public int TotalPayments { get; set; }
        public int CompletedPayments { get; set; }
        public int PendingPayments { get; set; }
        public int FailedPayments { get; set; }
        public decimal RefundedAmount { get; set; }
    }
}