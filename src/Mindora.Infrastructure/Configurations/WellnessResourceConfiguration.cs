using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class WellnessResourceConfiguration : IEntityTypeConfiguration<WellnessResource>
    {
        public void Configure(EntityTypeBuilder<WellnessResource> builder)
        {
            builder.HasKey(r => r.ResourceId);
            builder.Property(r => r.Title).IsRequired().HasMaxLength(300);
            builder.Property(r => r.IsPublished).HasDefaultValue(false);
        }
    }
}
