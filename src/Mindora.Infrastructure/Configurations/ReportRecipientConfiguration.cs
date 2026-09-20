using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ReportRecipientConfiguration : IEntityTypeConfiguration<ReportRecipient>
    {
        public void Configure(EntityTypeBuilder<ReportRecipient> builder)
        {
            builder.HasKey(r => r.ReportRecipientId);

            builder.Property(r => r.DeliveryChannel)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Email");

            builder.Property(r => r.AddedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(r => r.ScheduledReport)
                .WithMany(s => s.Recipients)
                .HasForeignKey(r => r.ScheduledReportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique: One user can be added once per scheduled report
            builder.HasIndex(r => new { r.ScheduledReportId, r.UserId })
                .IsUnique();
        }
    }
}