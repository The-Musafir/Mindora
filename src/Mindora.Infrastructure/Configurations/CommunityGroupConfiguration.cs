using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class CommunityGroupConfiguration : IEntityTypeConfiguration<CommunityGroup>
    {
        public void Configure(EntityTypeBuilder<CommunityGroup> builder)
        {
            builder.HasKey(g => g.GroupId);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.Description)
                .HasMaxLength(2000);

            builder.Property(g => g.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(g => g.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationships
            builder.HasOne(g => g.CreatedByUser)
                .WithMany()
                .HasForeignKey(g => g.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(g => g.Name);
            builder.HasQueryFilter(g => !g.IsDeleted);
        }
    }
}