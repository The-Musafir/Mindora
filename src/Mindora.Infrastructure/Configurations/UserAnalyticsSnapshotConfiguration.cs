using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserAnalyticsSnapshotConfiguration : IEntityTypeConfiguration<UserAnalyticsSnapshot>
    {
        public void Configure(EntityTypeBuilder<UserAnalyticsSnapshot> builder)
        {
            builder.HasKey(s => s.SnapshotId);

            builder.Property(s => s.SnapshotType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Data)
                .IsRequired();

            builder.Property(s => s.GeneratedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => new { s.UserId, s.SnapshotType, s.GeneratedAt });
        }
    }
}