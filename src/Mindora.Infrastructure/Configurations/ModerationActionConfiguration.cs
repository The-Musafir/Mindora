using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ModerationActionConfiguration : IEntityTypeConfiguration<ModerationAction>
    {
        public void Configure(EntityTypeBuilder<ModerationAction> builder)
        {
            builder.HasKey(a => a.ActionId);
            builder.HasOne(a => a.Flag)
                .WithMany(f => f.ModerationActions)
                .HasForeignKey(a => a.FlagId);
            builder.HasOne(a => a.AdminUser)
                .WithMany()
                .HasForeignKey(a => a.AdminUserId);
        }
    }
}
