using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class ScheduledReportService : IScheduledReportService
    {
        private readonly MindoraDbContext _context;
        private readonly IGeneratedReportService _generatedReportService;

        public ScheduledReportService(
            MindoraDbContext context,
            IGeneratedReportService generatedReportService)
        {
            _context = context;
            _generatedReportService = generatedReportService;
        }

        public async Task<IReadOnlyList<ScheduledReportDto>> GetAllScheduledReportsAsync()
        {
            var reports = await _context.ScheduledReports
                .Include(s => s.Template)
                .Include(s => s.CreatedByUser)
                .Include(s => s.Recipients)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return reports.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<ScheduledReportDto>> GetActiveScheduledReportsAsync()
        {
            var reports = await _context.ScheduledReports
                .Where(s => s.IsActive)
                .Include(s => s.Template)
                .Include(s => s.CreatedByUser)
                .Include(s => s.Recipients)
                .OrderBy(s => s.NextRunAt)
                .ToListAsync();

            return reports.Select(MapToDto).ToList();
        }

        public async Task<ScheduledReportDto?> GetScheduledReportByIdAsync(Guid scheduledReportId)
        {
            var report = await _context.ScheduledReports
                .Include(s => s.Template)
                .Include(s => s.CreatedByUser)
                .Include(s => s.Recipients)
                .FirstOrDefaultAsync(s => s.ScheduledReportId == scheduledReportId);

            return report == null ? null : MapToDto(report);
        }

        public async Task<Guid> CreateScheduledReportAsync(Guid userId, CreateScheduledReportRequest request)
        {
            var now = DateTime.UtcNow;
            var nextRun = CalculateNextRun(now, request.Frequency, request.ScheduledTime);

            var scheduled = new ScheduledReport
            {
                ScheduledReportId = Guid.NewGuid(),
                TemplateId = request.TemplateId,
                CreatedByUserId = userId,
                ReportType = request.ReportType,
                Format = request.Format,
                Frequency = request.Frequency,
                ScheduledTime = request.ScheduledTime,
                NextRunAt = nextRun,
                IsActive = request.IsActive,
                CreatedAt = now
            };

            _context.ScheduledReports.Add(scheduled);
            await _context.SaveChangesAsync();
            return scheduled.ScheduledReportId;
        }

        public async Task<bool> UpdateScheduledReportAsync(ScheduledReportDto dto)
        {
            var scheduled = await _context.ScheduledReports.FindAsync(dto.ScheduledReportId);
            if (scheduled == null) return false;

            scheduled.ReportType = dto.ReportType;
            scheduled.Format = dto.Format;
            scheduled.Frequency = dto.Frequency;
            scheduled.ScheduledTime = dto.ScheduledTime;
            scheduled.IsActive = dto.IsActive;
            scheduled.NextRunAt = CalculateNextRun(DateTime.UtcNow, dto.Frequency, dto.ScheduledTime);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleScheduledReportAsync(Guid scheduledReportId, bool isActive)
        {
            var scheduled = await _context.ScheduledReports.FindAsync(scheduledReportId);
            if (scheduled == null) return false;

            scheduled.IsActive = isActive;
            if (isActive)
                scheduled.NextRunAt = CalculateNextRun(DateTime.UtcNow, scheduled.Frequency, scheduled.ScheduledTime);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteScheduledReportAsync(Guid scheduledReportId)
        {
            var scheduled = await _context.ScheduledReports.FindAsync(scheduledReportId);
            if (scheduled == null) return false;

            _context.ScheduledReports.Remove(scheduled);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RunScheduledReportNowAsync(Guid scheduledReportId)
        {
            var scheduled = await _context.ScheduledReports
                .Include(s => s.CreatedByUser)
                .FirstOrDefaultAsync(s => s.ScheduledReportId == scheduledReportId);

            if (scheduled == null) return false;

            // Generate a report using IGeneratedReportService
            var request = new CreateGeneratedReportRequest
            {
                TemplateId = scheduled.TemplateId,
                ReportType = scheduled.ReportType,
                Format = scheduled.Format,
                FromDate = DateTime.UtcNow.AddDays(-30),
                ToDate = DateTime.UtcNow
            };

            await _generatedReportService.RequestReportAsync(scheduled.CreatedByUserId, request);

            scheduled.LastRunAt = DateTime.UtcNow;
            scheduled.NextRunAt = CalculateNextRun(DateTime.UtcNow, scheduled.Frequency, scheduled.ScheduledTime);
            await _context.SaveChangesAsync();

            return true;
        }

        private DateTime CalculateNextRun(DateTime from, string frequency, TimeSpan scheduledTime)
        {
            var today = from.Date;
            var candidate = today + scheduledTime;

            if (candidate <= from)
            {
                candidate = frequency switch
                {
                    "Weekly" => candidate.AddDays(7),
                    "Monthly" => candidate.AddMonths(1),
                    _ => candidate.AddDays(1)
                };
            }

            return candidate;
        }

        private ScheduledReportDto MapToDto(ScheduledReport s)
        {
            return new ScheduledReportDto
            {
                ScheduledReportId = s.ScheduledReportId,
                TemplateId = s.TemplateId,
                TemplateName = s.Template?.Name,
                CreatedByUserId = s.CreatedByUserId,
                CreatedByEmail = s.CreatedByUser?.Email ?? "",
                ReportType = s.ReportType,
                Format = s.Format,
                Frequency = s.Frequency,
                ScheduledTime = s.ScheduledTime,
                LastRunAt = s.LastRunAt,
                NextRunAt = s.NextRunAt,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                RecipientCount = s.Recipients?.Count ?? 0
            };
        }
    }
}