using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.AuditId);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
            builder.Property(a => a.TableName).IsRequired().HasMaxLength(200);
            builder.Property(a => a.RecordId).IsRequired().HasMaxLength(100);
            builder.Property(a => a.OldValues);
            builder.Property(a => a.NewValues);
            builder.Property(a => a.IpAddress).HasMaxLength(50);
            builder.Property(a => a.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.TableName);
        }
    }
}