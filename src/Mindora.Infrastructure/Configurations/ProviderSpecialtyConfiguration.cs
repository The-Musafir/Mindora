using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ProviderSpecialtyConfiguration : IEntityTypeConfiguration<ProviderSpecialty>
    {
        public void Configure(EntityTypeBuilder<ProviderSpecialty> builder)
        {
            builder.HasKey(s => s.SpecialtyId);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.HasIndex(s => s.Name)
                .IsUnique();
        }
    }
}