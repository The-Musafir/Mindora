using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.InvoiceId);

            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.IssuedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship
            builder.HasOne(i => i.Payment)
                .WithMany()
                .HasForeignKey(i => i.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(i => i.InvoiceNumber)
                .IsUnique();
            builder.HasIndex(i => i.PaymentId);
        }
    }
}