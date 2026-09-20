using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ProfessionalProviderConfiguration : IEntityTypeConfiguration<ProfessionalProvider>
    {
        public void Configure(EntityTypeBuilder<ProfessionalProvider> builder)
        {
            builder.HasKey(p => p.ProviderId);

            builder.Property(p => p.Bio)
                .HasMaxLength(2000);

            builder.Property(p => p.LicenseNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.IsVerified)
                .HasDefaultValue(false);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with User (One-to-One)
            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<ProfessionalProvider>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique UserId
            builder.HasIndex(p => p.UserId)
                .IsUnique();
        }
    }
}