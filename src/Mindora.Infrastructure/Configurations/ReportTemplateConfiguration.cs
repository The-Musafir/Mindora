using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
    {
        public void Configure(EntityTypeBuilder<ReportTemplate> builder)
        {
            builder.HasKey(t => t.TemplateId);

            builder.Property(t => t.TemplateKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Business");

            builder.Property(t => t.DefaultFormat)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("CSV");

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(t => t.TemplateKey).IsUnique();
            builder.HasIndex(t => t.Category);
        }
    }
}