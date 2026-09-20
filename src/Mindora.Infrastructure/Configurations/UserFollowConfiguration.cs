using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.HasKey(f => f.FollowId);

            // Unique: এক user এক user কে একবারই follow করতে পারবে
            builder.HasIndex(f => new { f.FollowerId, f.FollowingId })
                .IsUnique();

            // Prevent self-follow at DB level (with check constraint)
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_UserFollow_NoSelfFollow",
                "\"FollowerId\" <> \"FollowingId\""));

            // Follower relationship
            builder.HasOne(f => f.Follower)
                .WithMany()
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Following relationship
            builder.HasOne(f => f.Following)
                .WithMany()
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Indexes for performance
            builder.HasIndex(f => f.FollowerId);
            builder.HasIndex(f => f.FollowingId);
        }
    }
}