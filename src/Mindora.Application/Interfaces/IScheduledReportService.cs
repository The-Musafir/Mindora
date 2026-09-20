using Mindora.Application.DTOs.Reporting;

namespace Mindora.Application.Interfaces
{
    public interface IScheduledReportService
    {
        Task<IReadOnlyList<ScheduledReportDto>> GetAllScheduledReportsAsync();
        Task<IReadOnlyList<ScheduledReportDto>> GetActiveScheduledReportsAsync();
        Task<ScheduledReportDto?> GetScheduledReportByIdAsync(Guid scheduledReportId);
        Task<Guid> CreateScheduledReportAsync(Guid userId, CreateScheduledReportRequest request);
        Task<bool> UpdateScheduledReportAsync(ScheduledReportDto dto);
        Task<bool> ToggleScheduledReportAsync(Guid scheduledReportId, bool isActive);
        Task<bool> DeleteScheduledReportAsync(Guid scheduledReportId);
        Task<bool> RunScheduledReportNowAsync(Guid scheduledReportId);
    }
}