using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class FollowUpPlanConfiguration : IEntityTypeConfiguration<FollowUpPlan>
    {
        public void Configure(EntityTypeBuilder<FollowUpPlan> builder)
        {
            builder.HasKey(p => p.FollowUpPlanId);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(p => p.IsCompleted)
                .HasDefaultValue(false);

            // Relationship
            builder.HasOne(p => p.Session)
                .WithMany(s => s.FollowUpPlans)
                .HasForeignKey(p => p.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(p => p.SessionId);
        }
    }
}