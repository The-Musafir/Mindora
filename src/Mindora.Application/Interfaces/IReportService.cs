using Mindora.Application.DTOs.Analytics;

namespace Mindora.Application.Interfaces
{
    public interface IReportService
    {
        Task<Guid> RequestReportAsync(Guid userId, CreateReportRequestDto request);
        Task<IReadOnlyList<ReportRequestDto>> GetUserReportsAsync(Guid userId);
        Task<IReadOnlyList<ReportRequestDto>> GetAllReportsAsync();
        Task<ReportRequestDto?> GetReportByIdAsync(Guid reportId);
        Task<bool> UpdateReportStatusAsync(Guid reportId, string status, string? fileUrl = null);
        Task<byte[]> GenerateUserAnalyticsCsvAsync(Guid userId, DateTime? fromDate, DateTime? toDate);
        Task<byte[]> GenerateAdminAnalyticsCsvAsync(DateTime? fromDate, DateTime? toDate);
    }
}