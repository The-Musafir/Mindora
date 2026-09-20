using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.CouponId);
            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.Property(c => c.DiscountType).IsRequired().HasMaxLength(20);
            builder.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
            builder.Property(c => c.ExpiryDate);
            builder.Property(c => c.UsageLimit).IsRequired();
            builder.Property(c => c.UsageCount).HasDefaultValue(0);
            builder.Property(c => c.IsActive).HasDefaultValue(true);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.HasIndex(c => c.Code).IsUnique();
        }
    }
}