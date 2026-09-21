using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class BoredomRecoveryActivityConfiguration : IEntityTypeConfiguration<BoredomRecoveryActivity>
    {
        public void Configure(EntityTypeBuilder<BoredomRecoveryActivity> builder)
        {
            builder.HasKey(a => a.ActivityId);
            builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Category).IsRequired().HasMaxLength(50);
            builder.Property(a => a.Emoji).HasMaxLength(10);
            builder.Property(a => a.Trivia).HasMaxLength(500);
            builder.Property(a => a.IsActive).HasDefaultValue(true);
        }
    }
}
