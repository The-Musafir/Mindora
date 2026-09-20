using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class SereneRecoverySessionConfiguration : IEntityTypeConfiguration<SereneRecoverySession>
    {
        public void Configure(EntityTypeBuilder<SereneRecoverySession> builder)
        {
            builder.HasKey(s => s.SessionId);
            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);
            builder.HasOne(s => s.Resource)
                .WithMany(r => r.Sessions)
                .HasForeignKey(s => s.ResourceId);
        }
    }
}
