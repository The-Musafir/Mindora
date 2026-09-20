using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ScheduledReportConfiguration : IEntityTypeConfiguration<ScheduledReport>
    {
        public void Configure(EntityTypeBuilder<ScheduledReport> builder)
        {
            builder.HasKey(s => s.ScheduledReportId);

            builder.Property(s => s.ReportType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Format)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("CSV");

            builder.Property(s => s.Frequency)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Daily");

            builder.Property(s => s.ScheduledTime)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(s => s.Template)
                .WithMany(t => t.ScheduledReports)
                .HasForeignKey(s => s.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(s => s.IsActive);
            builder.HasIndex(s => s.NextRunAt);
        }
    }
}