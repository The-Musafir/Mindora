using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class CommunityCommentConfiguration : IEntityTypeConfiguration<CommunityComment>
    {
        public void Configure(EntityTypeBuilder<CommunityComment> builder)
        {
            builder.HasKey(c => c.CommentId);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(c => c.PostId);
            builder.HasIndex(c => c.UserId);
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}