using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class BoxBreathingSessionConfiguration : IEntityTypeConfiguration<BoxBreathingSession>
    {
        public void Configure(EntityTypeBuilder<BoxBreathingSession> builder)
        {
            builder.HasKey(b => b.SessionId);
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);
        }
    }
}
