using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class WellnessResourceCategoryMappingConfiguration : IEntityTypeConfiguration<WellnessResourceCategoryMapping>
    {
        public void Configure(EntityTypeBuilder<WellnessResourceCategoryMapping> builder)
        {
            builder.HasKey(m => new { m.ResourceId, m.CategoryId });
            builder.HasOne(m => m.Resource)
                .WithMany(r => r.CategoryMappings)
                .HasForeignKey(m => m.ResourceId);
            builder.HasOne(m => m.Category)
                .WithMany(c => c.ResourceMappings)
                .HasForeignKey(m => m.CategoryId);
        }
    }
}
