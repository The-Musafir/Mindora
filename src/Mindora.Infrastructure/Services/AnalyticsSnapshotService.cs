using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AnalyticsSnapshotService : IAnalyticsSnapshotService
    {
        private readonly MindoraDbContext _context;
        private readonly IAdminAnalyticsFullService _adminAnalyticsService;
        private readonly IUserAnalyticsService _userAnalyticsService;

        public AnalyticsSnapshotService(
            MindoraDbContext context,
            IAdminAnalyticsFullService adminAnalyticsService,
            IUserAnalyticsService userAnalyticsService)
        {
            _context = context;
            _adminAnalyticsService = adminAnalyticsService;
            _userAnalyticsService = userAnalyticsService;
        }

        public async Task GeneratePlatformSnapshotAsync()
        {
            var analytics = await _adminAnalyticsService.GetFullAnalyticsAsync();

            var snapshot = new PlatformAnalyticsSnapshot
            {
                SnapshotId = Guid.NewGuid(),
                SnapshotType = "DailyPlatformSnapshot",
                Data = JsonSerializer.Serialize(analytics),
                GeneratedAt = DateTime.UtcNow
            };

            _context.PlatformAnalyticsSnapshots.Add(snapshot);
            await _context.SaveChangesAsync();
        }

        public async Task GenerateUserSnapshotAsync(Guid userId)
        {
            var analytics = await _userAnalyticsService.GetUserAnalyticsAsync(userId);

            var snapshot = new UserAnalyticsSnapshot
            {
                SnapshotId = Guid.NewGuid(),
                UserId = userId,
                SnapshotType = "DailyUserSnapshot",
                Data = JsonSerializer.Serialize(analytics),
                GeneratedAt = DateTime.UtcNow
            };

            _context.UserAnalyticsSnapshots.Add(snapshot);
            await _context.SaveChangesAsync();
        }
    }
}