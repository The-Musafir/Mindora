using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class CommunityReactionConfiguration : IEntityTypeConfiguration<CommunityReaction>
    {
        public void Configure(EntityTypeBuilder<CommunityReaction> builder)
        {
            builder.HasKey(r => r.ReactionId);

            builder.Property(r => r.ReactionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(r => r.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(r => new { r.UserId, r.PostId });
            builder.HasIndex(r => new { r.UserId, r.CommentId });
        }
    }
}