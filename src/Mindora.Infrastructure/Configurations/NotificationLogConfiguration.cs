using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
    {
        public void Configure(EntityTypeBuilder<NotificationLog> builder)
        {
            builder.HasKey(l => l.LogId);

            builder.Property(l => l.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Sent");

            builder.Property(l => l.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(l => l.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship (One-to-One with UserNotification)
            builder.HasOne(l => l.Notification)
                .WithOne(n => n.Log)
                .HasForeignKey<NotificationLog>(l => l.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(l => l.NotificationId)
                .IsUnique();
        }
    }
}