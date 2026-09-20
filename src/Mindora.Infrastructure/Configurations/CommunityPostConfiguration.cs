using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class CommunityPostConfiguration : IEntityTypeConfiguration<CommunityPost>
    {
        public void Configure(EntityTypeBuilder<CommunityPost> builder)
        {
            builder.HasKey(p => p.PostId);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.Body)
                .IsRequired();

            builder.Property(p => p.Category)
                .HasMaxLength(100);

            builder.Property(p => p.IsAnonymous)
                .HasDefaultValue(false);

            builder.Property(p => p.IsPinned)
                .HasDefaultValue(false);

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Group)
                .WithMany(g => g.Posts)
                .HasForeignKey(p => p.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.GroupId);
            builder.HasIndex(p => p.IsPinned);
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}