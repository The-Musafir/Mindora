using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class GeneratedReportService : IGeneratedReportService
    {
        private readonly MindoraDbContext _context;
        private readonly IReportExportService _exportService;

        public GeneratedReportService(
            MindoraDbContext context,
            IReportExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        public async Task<IReadOnlyList<GeneratedReportDto>> GetUserReportsAsync(Guid userId)
        {
            var reports = await _context.GeneratedReports
                .Where(r => r.RequestedByUserId == userId)
                .Include(r => r.Template)
                .Include(r => r.RequestedByUser)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return reports.Select(MapToDto).ToList();
        }

        public async Task<GeneratedReportDto?> GetReportByIdAsync(Guid reportId)
        {
            var report = await _context.GeneratedReports
                .Include(r => r.Template)
                .Include(r => r.RequestedByUser)
                .FirstOrDefaultAsync(r => r.GeneratedReportId == reportId);

            return report == null ? null : MapToDto(report);
        }

        public async Task<IReadOnlyList<GeneratedReportDto>> GetAllReportsAsync()
        {
            var reports = await _context.GeneratedReports
                .Include(r => r.Template)
                .Include(r => r.RequestedByUser)
                .OrderByDescending(r => r.RequestedAt)
                .Take(500)
                .ToListAsync();

            return reports.Select(MapToDto).ToList();
        }

        public async Task<Guid> RequestReportAsync(Guid userId, CreateGeneratedReportRequest request)
        {
            var report = new GeneratedReport
            {
                GeneratedReportId = Guid.NewGuid(),
                TemplateId = request.TemplateId,
                RequestedByUserId = userId,
                ReportType = request.ReportType,
                Format = request.Format,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _context.GeneratedReports.Add(report);
            await _context.SaveChangesAsync();

            // Auto-generate the file immediately (synchronous for MVP)
            try
            {
                var filter = new ReportFilterDto
                {
                    ReportType = report.ReportType,
                    FromDate = report.FromDate,
                    ToDate = report.ToDate,
                    Format = report.Format
                };

                byte[] fileBytes = report.Format switch
                {
                    "PDF" => await _exportService.ExportToPdfAsync(filter),
                    "Excel" => await _exportService.ExportToExcelAsync(filter),
                    _ => await _exportService.ExportToCsvAsync(filter)
                };

                report.FileSizeBytes = fileBytes.Length;
                report.Status = "Completed";
                report.CompletedAt = DateTime.UtcNow;
                // FileUrl: for MVP we won't store file; will regenerate on download
                report.FileUrl = null;

                await _context.SaveChangesAsync();
            }
            catch
            {
                report.Status = "Failed";
                await _context.SaveChangesAsync();
            }

            return report.GeneratedReportId;
        }

        public async Task<bool> UpdateReportStatusAsync(Guid reportId, string status, string? fileUrl = null, long? fileSizeBytes = null)
        {
            var report = await _context.GeneratedReports.FindAsync(reportId);
            if (report == null) return false;

            report.Status = status;
            if (!string.IsNullOrWhiteSpace(fileUrl)) report.FileUrl = fileUrl;
            if (fileSizeBytes.HasValue) report.FileSizeBytes = fileSizeBytes;
            if (status == "Completed") report.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReportAsync(Guid reportId)
        {
            var report = await _context.GeneratedReports.FindAsync(reportId);
            if (report == null) return false;

            _context.GeneratedReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> DownloadReportAsync(Guid reportId, Guid requestingUserId, bool isAdmin = false)
        {
            var report = await _context.GeneratedReports.FindAsync(reportId);
            if (report == null)
                throw new KeyNotFoundException("Report not found.");

            if (!isAdmin && report.RequestedByUserId != requestingUserId)
                throw new UnauthorizedAccessException("You are not authorized to download this report.");

            var filter = new ReportFilterDto
            {
                ReportType = report.ReportType,
                FromDate = report.FromDate,
                ToDate = report.ToDate,
                Format = report.Format
            };

            return report.Format switch
            {
                "PDF" => await _exportService.ExportToPdfAsync(filter),
                "Excel" => await _exportService.ExportToExcelAsync(filter),
                _ => await _exportService.ExportToCsvAsync(filter)
            };
        }

        private GeneratedReportDto MapToDto(GeneratedReport r)
        {
            return new GeneratedReportDto
            {
                GeneratedReportId = r.GeneratedReportId,
                TemplateId = r.TemplateId,
                TemplateName = r.Template?.Name,
                RequestedByUserId = r.RequestedByUserId,
                RequestedByEmail = r.RequestedByUser?.Email ?? "",
                ReportType = r.ReportType,
                Format = r.Format,
                FromDate = r.FromDate,
                ToDate = r.ToDate,
                Status = r.Status,
                FileUrl = r.FileUrl,
                FileSizeBytes = r.FileSizeBytes,
                RequestedAt = r.RequestedAt,
                CompletedAt = r.CompletedAt
            };
        }
    }
}