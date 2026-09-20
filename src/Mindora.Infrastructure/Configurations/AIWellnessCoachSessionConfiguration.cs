using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AIWellnessCoachSessionConfiguration : IEntityTypeConfiguration<AIWellnessCoachSession>
    {
        public void Configure(EntityTypeBuilder<AIWellnessCoachSession> builder)
        {
            builder.HasKey(s => s.SessionId);

            builder.Property(s => s.StartedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            builder.Property(s => s.LastContext)
                .HasMaxLength(4000);

            // Relationship with User
            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => s.IsActive);
        }
    }
}