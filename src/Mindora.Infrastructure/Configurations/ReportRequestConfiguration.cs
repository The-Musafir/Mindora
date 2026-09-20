using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ReportRequestConfiguration : IEntityTypeConfiguration<ReportRequest>
    {
        public void Configure(EntityTypeBuilder<ReportRequest> builder)
        {
            builder.HasKey(r => r.ReportId);

            builder.Property(r => r.ReportType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Format)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("CSV");

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(r => r.FileUrl)
                .HasMaxLength(500);

            builder.Property(r => r.RequestedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(r => r.RequestedByUser)
                .WithMany()
                .HasForeignKey(r => r.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Template)
                .WithMany()
                .HasForeignKey(r => r.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(r => r.RequestedByUserId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => new { r.ReportType, r.RequestedAt });
        }
    }
}