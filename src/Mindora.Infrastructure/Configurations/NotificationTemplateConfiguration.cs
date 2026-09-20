using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.HasKey(t => t.TemplateId);

            builder.Property(t => t.TemplateKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(t => t.BodyTemplate)
                .IsRequired();

            builder.Property(t => t.Channel)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("InApp");

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(t => t.TemplateKey)
                .IsUnique();
        }
    }
}