using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class CommunityGroupMemberConfiguration : IEntityTypeConfiguration<CommunityGroupMember>
    {
        public void Configure(EntityTypeBuilder<CommunityGroupMember> builder)
        {
            builder.HasKey(cgm => new { cgm.GroupId, cgm.UserId });

            builder.Property(cgm => cgm.JoinedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(cgm => cgm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(cgm => cgm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cgm => cgm.User)
                .WithMany()
                .HasForeignKey(cgm => cgm.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}