using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
    {
        public void Configure(EntityTypeBuilder<UserSubscription> builder)
        {
            builder.HasKey(s => s.SubscriptionId);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            builder.Property(s => s.StartDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(s => s.EndDate)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Plan)
                .WithMany(p => p.UserSubscriptions)
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => new { s.UserId, s.Status });
        }
    }
}