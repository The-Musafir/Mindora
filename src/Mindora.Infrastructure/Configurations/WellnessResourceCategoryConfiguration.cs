using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class WellnessResourceCategoryConfiguration : IEntityTypeConfiguration<WellnessResourceCategory>
    {
        public void Configure(EntityTypeBuilder<WellnessResourceCategory> builder)
        {
            builder.HasKey(c => c.CategoryId);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        }
    }
}
