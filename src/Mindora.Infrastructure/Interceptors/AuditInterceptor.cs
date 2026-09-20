using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Mindora.Domain.Entities;
using System.Text.Json;

namespace Mindora.Infrastructure.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            var context = eventData.Context;
            if (context != null)
            {
                AddAuditLogs(context);
            }
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context != null)
            {
                AddAuditLogs(context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AddAuditLogs(DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .Where(e => e.Metadata.ClrType != typeof(AuditLog))
                .Where(e => e.Metadata.ClrType != typeof(UserLoginHistory))
                .ToList();

            foreach (var entry in entries)
            {
                var audit = new AuditLog
                {
                    Action = entry.State.ToString(),
                    TableName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name,
                    RecordId = GetRecordId(entry),
                    OldValues = entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                        ? JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p.Name]))
                        : null,
                    NewValues = entry.State == EntityState.Added || entry.State == EntityState.Modified
                        ? JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name]))
                        : null,
                    Timestamp = DateTime.UtcNow
                };

                context.Add(audit);
            }
        }

        private string GetRecordId(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key == null) return string.Empty;

            var keyValues = key.Properties
                .Select(p => entry.Property(p.Name).CurrentValue ?? entry.Property(p.Name).OriginalValue)
                .Select(v => v?.ToString())
                .ToArray();

            return string.Join("_", keyValues);
        }
    }
}