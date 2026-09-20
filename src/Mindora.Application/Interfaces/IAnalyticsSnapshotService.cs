using Mindora.Application.DTOs.AdminDashboard;

namespace Mindora.Application.Interfaces
{
    public interface IAnalyticsSnapshotService
    {
        Task GeneratePlatformSnapshotAsync();
        Task GenerateUserSnapshotAsync(Guid userId);
    }
}