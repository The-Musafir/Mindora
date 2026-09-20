using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AppointmentStatusHistoryConfiguration : IEntityTypeConfiguration<AppointmentStatusHistory>
    {
        public void Configure(EntityTypeBuilder<AppointmentStatusHistory> builder)
        {
            builder.HasKey(h => h.HistoryId);

            builder.Property(h => h.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(h => h.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(h => h.Appointment)
                .WithMany(a => a.StatusHistories)
                .HasForeignKey(h => h.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}