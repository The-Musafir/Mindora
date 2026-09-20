using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ProviderVerificationDocumentConfiguration : IEntityTypeConfiguration<ProviderVerificationDocument>
    {
        public void Configure(EntityTypeBuilder<ProviderVerificationDocument> builder)
        {
            builder.HasKey(d => d.DocumentId);

            builder.Property(d => d.DocumentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.HasOne(d => d.Provider)
                .WithMany(p => p.VerificationDocuments)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}