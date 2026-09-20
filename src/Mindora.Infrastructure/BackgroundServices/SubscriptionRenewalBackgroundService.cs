using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mindora.Application.Interfaces;

namespace Mindora.Infrastructure.BackgroundServices
{
    public class SubscriptionRenewalBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubscriptionRenewalBackgroundService> _logger;

        public SubscriptionRenewalBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<SubscriptionRenewalBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Renewal Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var renewalService = scope.ServiceProvider
                        .GetRequiredService<ISubscriptionRenewalService>();

                    int renewedCount = await renewalService.RenewExpiredSubscriptionsAsync();

                    if (renewedCount > 0)
                        _logger.LogInformation("Auto renewed {Count} subscriptions.", renewedCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in subscription renewal background job.");
                }

                // প্রতি ২৪ ঘণ্টা পর চালান
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}