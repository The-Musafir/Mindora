using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ProviderSpecialtyMappingConfiguration : IEntityTypeConfiguration<ProviderSpecialtyMapping>
    {
        public void Configure(EntityTypeBuilder<ProviderSpecialtyMapping> builder)
        {
            builder.HasKey(psm => new { psm.ProviderId, psm.SpecialtyId });

            builder.HasOne(psm => psm.Provider)
                .WithMany(p => p.SpecialtyMappings)
                .HasForeignKey(psm => psm.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(psm => psm.Specialty)
                .WithMany(s => s.ProviderMappings)
                .HasForeignKey(psm => psm.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}