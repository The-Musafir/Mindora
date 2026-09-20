using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminAuditService : IAdminAuditService
    {
        private readonly MindoraDbContext _context;

        public AdminAuditService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AdminAuditLogDto>> GetAuditLogsAsync()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .ToListAsync();

            return logs.Select(l => new AdminAuditLogDto
            {
                AuditId = l.AuditId,
                UserId = l.UserId,
                UserEmail = l.UserId.HasValue
                    ? _context.Users.FirstOrDefault(u => u.Id == l.UserId.Value)?.Email ?? ""
                    : "System",
                Action = l.Action,
                TableName = l.TableName,
                RecordId = l.RecordId,
                IpAddress = l.IpAddress,
                Timestamp = l.Timestamp
            }).ToList();
        }

        public async Task<IReadOnlyList<AdminLoginHistoryDto>> GetLoginHistoriesAsync()
        {
            var histories = await _context.UserLoginHistories
                .OrderByDescending(h => h.LoginAt)
                .Take(100)
                .ToListAsync();

            return histories.Select(h => new AdminLoginHistoryDto
            {
                LoginHistoryId = h.LoginHistoryId,
                UserId = h.UserId,
                UserEmail = _context.Users.FirstOrDefault(u => u.Id == h.UserId)?.Email ?? "",
                LoginAt = h.LoginAt,
                IpAddress = h.IpAddress,
                UserAgent = h.UserAgent,
                IsSuccess = h.IsSuccess
            }).ToList();
        }
    }
}