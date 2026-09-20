using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class BoredomRecoverySessionConfiguration : IEntityTypeConfiguration<BoredomRecoverySession>
    {
        public void Configure(EntityTypeBuilder<BoredomRecoverySession> builder)
        {
            builder.HasKey(s => s.SessionId);
            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);
            builder.HasOne(s => s.Activity)
                .WithMany(a => a.Sessions)
                .HasForeignKey(s => s.ActivityId);
        }
    }
}
