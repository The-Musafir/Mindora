using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ProviderPracticeDetailConfiguration : IEntityTypeConfiguration<ProviderPracticeDetail>
    {
        public void Configure(EntityTypeBuilder<ProviderPracticeDetail> builder)
        {
            builder.HasKey(d => d.PracticeId);

            builder.Property(d => d.PracticeName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Address)
                .HasMaxLength(500);

            builder.Property(d => d.Phone)
                .HasMaxLength(50);

            builder.Property(d => d.IsVirtual)
                .HasDefaultValue(false);

            builder.HasOne(d => d.Provider)
                .WithMany(p => p.PracticeDetails)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}