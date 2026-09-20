using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly MindoraDbContext _context;

        public AuditService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(Guid? userId, string action, string tableName, string recordId, string? oldValues = null, string? newValues = null)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
    }
}