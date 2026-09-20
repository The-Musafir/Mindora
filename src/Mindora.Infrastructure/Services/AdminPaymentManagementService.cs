using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminPaymentManagementService : IAdminPaymentManagementService
    {
        private readonly MindoraDbContext _context;

        public AdminPaymentManagementService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<AdminPaymentManagementDto> GetManagementDataAsync()
        {
            return new AdminPaymentManagementDto
            {
                Payments = (await GetAllPaymentsAsync()).ToList(),
                Refunds = (await GetAllRefundsAsync()).ToList(),
                Payouts = (await GetAllPayoutsAsync()).ToList(),
                Coupons = (await GetAllCouponsAsync()).ToList()
            };
        }

        public async Task<IReadOnlyList<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return payments.Select(p => new PaymentResponseDto
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
            }).ToList();
        }

        public async Task<IReadOnlyList<RefundRequestDto>> GetAllRefundsAsync()
        {
            var refunds = await _context.RefundRequests
                .Include(r => r.Payment)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return refunds.Select(r => new RefundRequestDto
            {
                RefundId = r.RefundId,
                PaymentId = r.PaymentId,
                UserId = r.UserId,
                UserEmail = r.User.Email,
                Amount = r.Payment.Amount,
                Reason = r.Reason,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ReviewedAt = r.ReviewedAt
            }).ToList();
        }

        public async Task<IReadOnlyList<PayoutDto>> GetAllPayoutsAsync()
        {
            var payouts = await _context.Payouts
                .Include(p => p.Provider.User)
                .OrderByDescending(p => p.RequestedAt)
                .ToListAsync();

            return payouts.Select(p => new PayoutDto
            {
                PayoutId = p.PayoutId,
                ProviderId = p.ProviderId,
                ProviderName = p.Provider.User.UserName ?? p.Provider.User.Email,
                Amount = p.Amount,
                Method = p.Method,
                AccountDetails = p.AccountDetails,
                Status = p.Status,
                RequestedAt = p.RequestedAt,
                ReviewedAt = p.ReviewedAt
            }).ToList();
        }

        public async Task<IReadOnlyList<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _context.Coupons
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return coupons.Select(c => new CouponDto
            {
                CouponId = c.CouponId,
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                ExpiryDate = c.ExpiryDate,
                UsageLimit = c.UsageLimit,
                UsageCount = c.UsageCount,
                IsActive = c.IsActive
            }).ToList();
        }

        public async Task<bool> UpdateRefundStatusAsync(Guid refundId, string status, Guid adminUserId)
        {
            var refund = await _context.RefundRequests.FindAsync(refundId);
            if (refund == null || refund.Status != "Pending") return false;

            refund.Status = status;
            refund.ReviewedAt = DateTime.UtcNow;
            refund.ReviewedBy = adminUserId;

            if (status == "Approved")
            {
                var payment = await _context.Payments.FindAsync(refund.PaymentId);
                if (payment != null) payment.Status = "Refunded";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePayoutStatusAsync(Guid payoutId, string status, Guid adminUserId)
        {
            var payout = await _context.Payouts.FindAsync(payoutId);
            if (payout == null || payout.Status != "Pending") return false;

            payout.Status = status;
            payout.ReviewedAt = DateTime.UtcNow;
            payout.ReviewedBy = adminUserId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleCouponActiveAsync(Guid couponId, bool isActive)
        {
            var coupon = await _context.Coupons.FindAsync(couponId);
            if (coupon == null) return false;

            coupon.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}