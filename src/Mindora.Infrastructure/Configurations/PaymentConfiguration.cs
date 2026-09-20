using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.PaymentId);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("BDT");

            builder.Property(p => p.Gateway)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(p => p.PaymentType)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Subscription");

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(p => p.IdempotencyKey)
                .IsUnique()
                .HasFilter("\"IdempotencyKey\" IS NOT NULL");

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Subscription)
                .WithMany()
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.ConsultationPayment)
                .WithOne(cp => cp.Payment)
                .HasForeignKey<Payment>(p => p.ConsultationPaymentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.TransactionId);
            builder.HasIndex(p => p.Status);
        }
    }
}