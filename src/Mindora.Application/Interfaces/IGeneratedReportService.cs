using Mindora.Application.DTOs.Reporting;

namespace Mindora.Application.Interfaces
{
    public interface IGeneratedReportService
    {
        // User-facing
        Task<IReadOnlyList<GeneratedReportDto>> GetUserReportsAsync(Guid userId);
        Task<GeneratedReportDto?> GetReportByIdAsync(Guid reportId);

        // Admin-facing
        Task<IReadOnlyList<GeneratedReportDto>> GetAllReportsAsync();

        // Create & Process
        Task<Guid> RequestReportAsync(Guid userId, CreateGeneratedReportRequest request);
        Task<bool> UpdateReportStatusAsync(Guid reportId, string status, string? fileUrl = null, long? fileSizeBytes = null);
        Task<bool> DeleteReportAsync(Guid reportId);

        // Download
        Task<byte[]> DownloadReportAsync(Guid reportId, Guid requestingUserId, bool isAdmin = false);
    }
}