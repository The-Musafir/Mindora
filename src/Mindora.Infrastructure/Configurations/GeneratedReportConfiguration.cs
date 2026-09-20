using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class GeneratedReportConfiguration : IEntityTypeConfiguration<GeneratedReport>
    {
        public void Configure(EntityTypeBuilder<GeneratedReport> builder)
        {
            builder.HasKey(g => g.GeneratedReportId);

            builder.Property(g => g.ReportType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.Format)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("CSV");

            builder.Property(g => g.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(g => g.FileUrl)
                .HasMaxLength(500);

            builder.Property(g => g.RequestedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(g => g.Template)
                .WithMany(t => t.GeneratedReports)
                .HasForeignKey(g => g.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(g => g.RequestedByUser)
                .WithMany()
                .HasForeignKey(g => g.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(g => g.RequestedByUserId);
            builder.HasIndex(g => g.Status);
            builder.HasIndex(g => new { g.ReportType, g.RequestedAt });
        }
    }
}