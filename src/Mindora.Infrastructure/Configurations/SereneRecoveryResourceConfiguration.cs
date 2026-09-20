using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class SereneRecoveryResourceConfiguration : IEntityTypeConfiguration<SereneRecoveryResource>
    {
        public void Configure(EntityTypeBuilder<SereneRecoveryResource> builder)
        {
            builder.HasKey(r => r.ResourceId);
            builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
        }
    }
}
