using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserSecuritySettingConfiguration : IEntityTypeConfiguration<UserSecuritySetting>
    {
        public void Configure(EntityTypeBuilder<UserSecuritySetting> builder)
        {
            builder.HasKey(uss => uss.SecuritySettingId); // Primary Key

            builder.HasOne(uss => uss.User)
                .WithOne(u => u.SecuritySetting)
                .HasForeignKey<UserSecuritySetting>(uss => uss.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(uss => uss.UserId).IsUnique();
        }
    }
}