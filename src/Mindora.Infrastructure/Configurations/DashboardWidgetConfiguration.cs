using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class DashboardWidgetConfiguration : IEntityTypeConfiguration<DashboardWidget>
    {
        public void Configure(EntityTypeBuilder<DashboardWidget> builder)
        {
            builder.HasKey(dw => dw.WidgetId);
            builder.HasOne(dw => dw.User)
                .WithMany()
                .HasForeignKey(dw => dw.UserId);
        }
    }
}
