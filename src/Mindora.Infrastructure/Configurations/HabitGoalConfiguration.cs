using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class HabitGoalConfiguration : IEntityTypeConfiguration<HabitGoal>
    {
        public void Configure(EntityTypeBuilder<HabitGoal> builder)
        {
            builder.HasKey(g => g.GoalId);
            builder.HasOne(g => g.Habit)
                .WithMany(h => h.Goals)
                .HasForeignKey(g => g.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(g => g.TargetValue).HasPrecision(10, 2);
        }
    }
}
