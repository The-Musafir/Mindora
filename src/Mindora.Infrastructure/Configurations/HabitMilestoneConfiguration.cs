using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class HabitMilestoneConfiguration : IEntityTypeConfiguration<HabitMilestone>
    {
        public void Configure(EntityTypeBuilder<HabitMilestone> builder)
        {
            builder.HasKey(m => m.MilestoneId);
            builder.HasOne(m => m.Habit)
                .WithMany(h => h.Milestones)
                .HasForeignKey(m => m.HabitId);
        }
    }
}
