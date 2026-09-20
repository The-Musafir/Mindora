using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class RevenueService : IRevenueService
    {
        private readonly MindoraDbContext _context;

        public RevenueService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var yearStart = new DateTime(now.Year, 1, 1);

            var payments = await _context.Payments
                .ToListAsync();

            var completedPayments = payments
                .Where(p => p.Status == "Completed")
                .ToList();

            var refundedPayments = payments
                .Where(p => p.Status == "Refunded")
                .ToList();

            return new RevenueAnalyticsDto
            {
                TotalRevenue = completedPayments.Sum(p => p.Amount),
                MonthlyRevenue = completedPayments
                    .Where(p => p.CreatedAt >= monthStart)
                    .Sum(p => p.Amount),
                YearlyRevenue = completedPayments
                    .Where(p => p.CreatedAt >= yearStart)
                    .Sum(p => p.Amount),
                SubscriptionRevenue = completedPayments
                    .Where(p => p.PaymentType == "Subscription")
                    .Sum(p => p.Amount),
                ConsultationRevenue = completedPayments
                    .Where(p => p.PaymentType == "Consultation")
                    .Sum(p => p.Amount),
                TotalPayments = payments.Count,
                CompletedPayments = completedPayments.Count,
                PendingPayments = payments.Count(p => p.Status == "Pending"),
                FailedPayments = payments.Count(p => p.Status == "Failed"),
                RefundedAmount = refundedPayments.Sum(p => p.Amount)
            };
        }
    }
}