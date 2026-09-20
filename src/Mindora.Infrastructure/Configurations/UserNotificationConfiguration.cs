using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(n => n.NotificationId);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(n => n.Body)
                .IsRequired();

            builder.Property(n => n.Channel)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("InApp");

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("General");

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Template)
                .WithMany(t => t.UserNotifications)
                .HasForeignKey(n => n.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index
            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => new { n.UserId, n.IsRead });
            builder.HasIndex(n => n.Type);
            builder.HasIndex(n => n.ReferenceId);
        }
    }
}