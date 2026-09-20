using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ProviderAvailabilitySlotConfiguration : IEntityTypeConfiguration<ProviderAvailabilitySlot>
    {
        public void Configure(EntityTypeBuilder<ProviderAvailabilitySlot> builder)
        {
            builder.HasKey(s => s.SlotId);

            builder.Property(s => s.DayOfWeek)
                .IsRequired();

            builder.Property(s => s.StartTime)
                .IsRequired();

            builder.Property(s => s.EndTime)
                .IsRequired();

            builder.Property(s => s.IsRecurring)
                .HasDefaultValue(true);

            builder.HasOne(s => s.Practice)
                .WithMany(p => p.AvailabilitySlots)
                .HasForeignKey(s => s.PracticeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}