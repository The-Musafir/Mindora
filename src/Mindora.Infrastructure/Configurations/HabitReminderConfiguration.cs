using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class HabitReminderConfiguration : IEntityTypeConfiguration<HabitReminder>
    {
        public void Configure(EntityTypeBuilder<HabitReminder> builder)
        {
            builder.HasKey(r => r.HabitReminderId);

            builder.Property(r => r.ReminderTime)
                .IsRequired();

            builder.Property(r => r.IsEnabled)
                .HasDefaultValue(true);

            // Relationship
            builder.HasOne(r => r.Habit)
                .WithMany(h => h.Reminders)
                .HasForeignKey(r => r.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}