using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class CommunityModerationFlagConfiguration : IEntityTypeConfiguration<CommunityModerationFlag>
    {
        public void Configure(EntityTypeBuilder<CommunityModerationFlag> builder)
        {
            builder.HasKey(f => f.FlagId);

            builder.Property(f => f.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(f => f.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(f => f.Post)
                .WithMany()
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.Comment)
                .WithMany()
                .HasForeignKey(f => f.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.ReportedByUser)
                .WithMany()
                .HasForeignKey(f => f.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}