using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mindora.Application.Interfaces;

namespace Mindora.Infrastructure.BackgroundServices
{
    public class AnalyticsSnapshotBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AnalyticsSnapshotBackgroundService> _logger;

        public AnalyticsSnapshotBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<AnalyticsSnapshotBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Analytics Snapshot Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // অপেক্ষা করুন পরবর্তী রাত ২টা পর্যন্ত
                    var now = DateTime.Now;
                    var nextRun = now.Date.AddDays(1).AddHours(2);
                    var delay = nextRun - now;

                    _logger.LogInformation("Next analytics snapshot scheduled at {Time}", nextRun);
                    await Task.Delay(delay, stoppingToken);

                    using var scope = _scopeFactory.CreateScope();
                    var snapshotService = scope.ServiceProvider
                        .GetRequiredService<IAnalyticsSnapshotService>();

                    // Platform snapshot তৈরি করুন
                    await snapshotService.GeneratePlatformSnapshotAsync();
                    _logger.LogInformation("Platform analytics snapshot generated at {Time}", DateTime.UtcNow);

                    // সব সক্রিয় ইউজারের snapshot তৈরি করুন
                    var dbContext = scope.ServiceProvider
                        .GetRequiredService<Mindora.Infrastructure.Persistence.DbContext.MindoraDbContext>();

                    var activeUserIds = dbContext.Users
                        .Where(u => u.IsActive && !u.IsDeleted)
                        .Select(u => u.Id)
                        .ToList();

                    int successCount = 0;
                    foreach (var userId in activeUserIds)
                    {
                        try
                        {
                            await snapshotService.GenerateUserSnapshotAsync(userId);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to generate snapshot for user {UserId}", userId);
                        }
                    }

                    _logger.LogInformation("User analytics snapshots generated for {Count} users.", successCount);
                }
                catch (TaskCanceledException)
                {
                    // Application shutting down — exit gracefully
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in analytics snapshot background service.");
                    // ৫ মিনিট অপেক্ষা করে আবার চেষ্টা করুন
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }

            _logger.LogInformation("Analytics Snapshot Background Service stopped.");
        }
    }
}