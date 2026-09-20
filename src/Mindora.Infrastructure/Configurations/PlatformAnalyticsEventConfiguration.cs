using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class PlatformAnalyticsEventConfiguration : IEntityTypeConfiguration<PlatformAnalyticsEvent>
    {
        public void Configure(EntityTypeBuilder<PlatformAnalyticsEvent> builder)
        {
            builder.HasKey(e => e.EventId);
            builder.Property(e => e.EventId).UseIdentityColumn();

            builder.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Payload);

            builder.Property(e => e.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(e => new { e.EventType, e.Timestamp });
            builder.HasIndex(e => e.UserId);
        }
    }
}