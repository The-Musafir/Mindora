using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class SubscriptionRenewalService : ISubscriptionRenewalService
    {
        private readonly MindoraDbContext _context;
        private readonly ILogger<SubscriptionRenewalService> _logger;

        public SubscriptionRenewalService(
            MindoraDbContext context,
            ILogger<SubscriptionRenewalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> RenewExpiredSubscriptionsAsync()
        {
            var now = DateTime.UtcNow;

            // যেসব Active subscription শেষ হয়েছে বা আজ শেষ হবে
            var expiredSubscriptions = await _context.UserSubscriptions
                .Where(s => s.Status == "Active" && s.EndDate <= now)
                .Include(s => s.Plan)
                .ToListAsync();

            int renewedCount = 0;

            foreach (var subscription in expiredSubscriptions)
            {
                try
                {
                    // নতুন মেয়াদ: আগের EndDate থেকে Plan.DurationDays যোগ
                    var newStart = subscription.EndDate;
                    var newEnd = newStart.AddDays(subscription.Plan.DurationDays);

                    // নতুন Payment তৈরি (আপাতত Manual gateway)
                    var payment = new Payment
                    {
                        PaymentId = Guid.NewGuid(),
                        UserId = subscription.UserId,
                        Amount = subscription.Plan.Price,
                        Currency = "BDT",
                        Gateway = "Manual",
                        TransactionId = Guid.NewGuid().ToString("N"),
                        Status = "Completed",
                        PaymentType = "Subscription",
                        SubscriptionId = subscription.SubscriptionId,
                        IdempotencyKey = Guid.NewGuid().ToString("N"),
                        PaymentMethod = "AutoRenewal",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Payments.Add(payment);

                    // Subscription আপডেট
                    subscription.StartDate = newStart;
                    subscription.EndDate = newEnd;
                    subscription.Status = "Active";
                    subscription.CreatedAt = DateTime.UtcNow;

                    renewedCount++;
                    _logger.LogInformation("Renewed subscription {SubscriptionId} until {EndDate}",
                        subscription.SubscriptionId, newEnd);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to renew subscription {SubscriptionId}",
                        subscription.SubscriptionId);
                }
            }

            if (renewedCount > 0)
                await _context.SaveChangesAsync();

            return renewedCount;
        }
    }
}