using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserNotificationPreferenceConfiguration : IEntityTypeConfiguration<UserNotificationPreference>
    {
        public void Configure(EntityTypeBuilder<UserNotificationPreference> builder)
        {
            builder.HasKey(p => p.PreferenceId);

            builder.Property(p => p.EmailEnabled)
                .HasDefaultValue(true);

            builder.Property(p => p.PushEnabled)
                .HasDefaultValue(true);

            builder.Property(p => p.InAppEnabled)
                .HasDefaultValue(true);

            builder.Property(p => p.QuietHoursEnabled)
                .HasDefaultValue(false);

            builder.Property(p => p.Frequency)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("RealTime");

            // Relationship (One-to-One with User)
            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<UserNotificationPreference>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.UserId)
                .IsUnique();
        }
    }
}