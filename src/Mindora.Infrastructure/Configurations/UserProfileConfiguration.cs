using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(up => up.ProfileId);

            builder.HasOne(up => up.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(up => up.UserId).IsUnique();

            builder.Property(up => up.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

           
            builder.Property(up => up.Bio)
                .HasMaxLength(1000);

            
            builder.Property(up => up.Gender)
                .HasMaxLength(20);

            builder.Property(up => up.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(up => up.Timezone)
                .HasDefaultValue("UTC");

            builder.Property(up => up.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}