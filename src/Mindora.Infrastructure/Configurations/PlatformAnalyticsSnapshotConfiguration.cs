using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class PlatformAnalyticsSnapshotConfiguration : IEntityTypeConfiguration<PlatformAnalyticsSnapshot>
    {
        public void Configure(EntityTypeBuilder<PlatformAnalyticsSnapshot> builder)
        {
            builder.HasKey(s => s.SnapshotId);

            builder.Property(s => s.SnapshotType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Data)
                .IsRequired();

            builder.Property(s => s.GeneratedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(s => new { s.SnapshotType, s.GeneratedAt });
        }
    }
}