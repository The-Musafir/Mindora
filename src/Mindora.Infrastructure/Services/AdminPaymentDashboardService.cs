using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminPaymentDashboardService : IAdminPaymentDashboardService
    {
        private readonly MindoraDbContext _context;

        public AdminPaymentDashboardService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<AdminPaymentDashboardDto> GetDashboardDataAsync()
        {
            var now = DateTime.UtcNow;

            // Summary counts
            var totalRevenue = await _context.Payments
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.Amount);

            var totalPayments = await _context.Payments.CountAsync();

            var pendingRefunds = await _context.RefundRequests
                .Where(r => r.Status == "Pending")
                .CountAsync();

            var activeSubscriptions = await _context.UserSubscriptions
                .CountAsync(s => s.Status == "Active" && s.EndDate > now);

            var pendingPayouts = await _context.Payouts
                .CountAsync(p => p.Status == "Pending");

            var activeCoupons = await _context.Coupons
                .CountAsync(c => c.IsActive);

            // Recent payments
            var recentPayments = await _context.Payments
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .Select(p => new PaymentResponseDto
                {
                    PaymentId = p.PaymentId,
                    UserId = p.UserId,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    Gateway = p.Gateway,
                    TransactionId = p.TransactionId,
                    Status = p.Status,
                    PaymentType = p.PaymentType,
                    PaymentMethod = p.PaymentMethod,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            // Pending refunds
            var pendingRefundList = await _context.RefundRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.Payment)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .Select(r => new RefundRequestDto
                {
                    RefundId = r.RefundId,
                    PaymentId = r.PaymentId,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    Amount = r.Payment.Amount,
                    Reason = r.Reason,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            // Pending payouts
            var pendingPayoutList = await _context.Payouts
                .Where(p => p.Status == "Pending")
                .Include(p => p.Provider.User)
                .OrderByDescending(p => p.RequestedAt)
                .Take(10)
                .Select(p => new PayoutDto
                {
                    PayoutId = p.PayoutId,
                    ProviderId = p.ProviderId,
                    ProviderName = p.Provider.User.UserName ?? p.Provider.User.Email,
                    Amount = p.Amount,
                    Method = p.Method,
                    AccountDetails = p.AccountDetails,
                    Status = p.Status,
                    RequestedAt = p.RequestedAt
                })
                .ToListAsync();

            // Active coupons
            var activeCouponList = await _context.Coupons
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .Take(10)
                .Select(c => new CouponDto
                {
                    CouponId = c.CouponId,
                    Code = c.Code,
                    DiscountType = c.DiscountType,
                    DiscountValue = c.DiscountValue,
                    ExpiryDate = c.ExpiryDate,
                    UsageLimit = c.UsageLimit,
                    UsageCount = c.UsageCount,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return new AdminPaymentDashboardDto
            {
                TotalRevenue = totalRevenue,
                TotalPayments = totalPayments,
                PendingRefundsCount = pendingRefunds,
                ActiveSubscriptionsCount = activeSubscriptions,
                PendingPayoutsCount = pendingPayouts,
                ActiveCouponsCount = activeCoupons,
                RecentPayments = recentPayments,
                PendingRefunds = pendingRefundList,
                PendingPayouts = pendingPayoutList,
                ActiveCoupons = activeCouponList
            };
        }
    }
}