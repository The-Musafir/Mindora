using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ConsultationPrescriptionConfiguration : IEntityTypeConfiguration<ConsultationPrescription>
    {
        public void Configure(EntityTypeBuilder<ConsultationPrescription> builder)
        {
            builder.HasKey(p => p.PrescriptionId);

            builder.Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship
            builder.HasOne(p => p.Session)
                .WithMany(s => s.Prescriptions)
                .HasForeignKey(p => p.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(p => p.SessionId);
        }
    }
}