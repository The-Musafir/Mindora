using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
    {
        public void Configure(EntityTypeBuilder<Payout> builder)
        {
            builder.HasKey(p => p.PayoutId);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Method)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Bank");

            builder.Property(p => p.AccountDetails)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(p => p.RequestedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(p => p.Provider)
                .WithMany()
                .HasForeignKey(p => p.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.ProviderId);
            builder.HasIndex(p => p.Status);
        }
    }
}