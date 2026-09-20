using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class RecoveryStreakConfiguration : IEntityTypeConfiguration<RecoveryStreak>
    {
        public void Configure(EntityTypeBuilder<RecoveryStreak> builder)
        {
            builder.HasKey(s => s.RecoveryStreakId);

            builder.Property(s => s.CurrentStreak)
                .HasDefaultValue(0);

            builder.Property(s => s.LongestStreak)
                .HasDefaultValue(0);

            builder.Property(s => s.StartDate)
                .IsRequired();

            builder.Property(s => s.EndDate);

            // Relationship
            builder.HasOne(s => s.Habit)
                .WithMany(h => h.Streaks)
                .HasForeignKey(s => s.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}