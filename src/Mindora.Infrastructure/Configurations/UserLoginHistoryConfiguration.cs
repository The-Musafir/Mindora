using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserLoginHistoryConfiguration : IEntityTypeConfiguration<UserLoginHistory>
    {
        public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
        {
            builder.HasKey(l => l.LoginHistoryId);
            builder.Property(l => l.LoginAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(l => l.IpAddress).HasMaxLength(50);
            builder.Property(l => l.UserAgent).HasMaxLength(500);
            builder.Property(l => l.IsSuccess).HasDefaultValue(true);
            builder.HasIndex(l => l.UserId);
        }
    }
}