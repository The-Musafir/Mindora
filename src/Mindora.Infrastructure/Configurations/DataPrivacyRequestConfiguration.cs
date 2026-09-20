using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class DataPrivacyRequestConfiguration : IEntityTypeConfiguration<DataPrivacyRequest>
    {
        public void Configure(EntityTypeBuilder<DataPrivacyRequest> builder)
        {
            builder.HasKey(dpr => dpr.RequestId);
            builder.HasOne(dpr => dpr.User)
                .WithMany(u => u.DataPrivacyRequests)
                .HasForeignKey(dpr => dpr.UserId);
            builder.Property(dpr => dpr.RequestType).IsRequired().HasMaxLength(50);
            builder.Property(dpr => dpr.Status).HasDefaultValue("Pending");
            builder.Property(dpr => dpr.RequestedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
