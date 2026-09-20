using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class PlatformSettingConfiguration : IEntityTypeConfiguration<PlatformSetting>
    {
        public void Configure(EntityTypeBuilder<PlatformSetting> builder)
        {
            builder.HasKey(s => s.SettingId);

            builder.Property(s => s.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Value)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(s => s.Key).IsUnique();
        }
    }
}