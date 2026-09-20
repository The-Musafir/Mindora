using Mindora.Application.DTOs.Reporting;

namespace Mindora.Application.Interfaces
{
    public interface IReportRecipientService
    {
        Task<IReadOnlyList<ReportRecipientDto>> GetRecipientsAsync(Guid scheduledReportId);
        Task<Guid> AddRecipientAsync(AddReportRecipientRequest request);
        Task<bool> RemoveRecipientAsync(Guid reportRecipientId);
        Task<bool> RemoveAllRecipientsAsync(Guid scheduledReportId);
    }
}