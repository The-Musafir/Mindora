using Mindora.Application.DTOs.Reporting;

namespace Mindora.Application.Interfaces
{
    public interface IReportExportService
    {
        // CSV
        Task<byte[]> ExportToCsvAsync(ReportFilterDto filter);

        // Excel
        Task<byte[]> ExportToExcelAsync(ReportFilterDto filter);

        // PDF
        Task<byte[]> ExportToPdfAsync(ReportFilterDto filter);
    }
}