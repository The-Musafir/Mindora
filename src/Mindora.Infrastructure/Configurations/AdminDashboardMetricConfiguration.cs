using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AdminDashboardMetricConfiguration : IEntityTypeConfiguration<AdminDashboardMetric>
    {
        public void Configure(EntityTypeBuilder<AdminDashboardMetric> builder)
        {
            builder.HasKey(m => m.MetricId);
            builder.Property(m => m.MetricKey).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Value).HasPrecision(18, 4);
        }
    }
}
