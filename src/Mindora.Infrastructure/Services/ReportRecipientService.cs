using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class ReportRecipientService : IReportRecipientService
    {
        private readonly MindoraDbContext _context;

        public ReportRecipientService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ReportRecipientDto>> GetRecipientsAsync(Guid scheduledReportId)
        {
            var recipients = await _context.ReportRecipients
                .Where(r => r.ScheduledReportId == scheduledReportId)
                .Include(r => r.User)
                .OrderBy(r => r.AddedAt)
                .ToListAsync();

            return recipients.Select(r => new ReportRecipientDto
            {
                ReportRecipientId = r.ReportRecipientId,
                ScheduledReportId = r.ScheduledReportId,
                UserId = r.UserId,
                UserEmail = r.User?.Email ?? "",
                DeliveryChannel = r.DeliveryChannel,
                AddedAt = r.AddedAt
            }).ToList();
        }

        public async Task<Guid> AddRecipientAsync(AddReportRecipientRequest request)
        {
            var exists = await _context.ReportRecipients
                .AnyAsync(r => r.ScheduledReportId == request.ScheduledReportId &&
                               r.UserId == request.UserId);

            if (exists)
                throw new InvalidOperationException("This user is already a recipient.");

            var recipient = new ReportRecipient
            {
                ReportRecipientId = Guid.NewGuid(),
                ScheduledReportId = request.ScheduledReportId,
                UserId = request.UserId,
                DeliveryChannel = request.DeliveryChannel,
                AddedAt = DateTime.UtcNow
            };

            _context.ReportRecipients.Add(recipient);
            await _context.SaveChangesAsync();
            return recipient.ReportRecipientId;
        }

        public async Task<bool> RemoveRecipientAsync(Guid reportRecipientId)
        {
            var recipient = await _context.ReportRecipients.FindAsync(reportRecipientId);
            if (recipient == null) return false;

            _context.ReportRecipients.Remove(recipient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAllRecipientsAsync(Guid scheduledReportId)
        {
            var recipients = await _context.ReportRecipients
                .Where(r => r.ScheduledReportId == scheduledReportId)
                .ToListAsync();

            if (!recipients.Any()) return false;

            _context.ReportRecipients.RemoveRange(recipients);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}