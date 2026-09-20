using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class HabitConfiguration : IEntityTypeConfiguration<Habit>
    {
        public void Configure(EntityTypeBuilder<Habit> builder)
        {
            builder.HasKey(h => h.HabitId);

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Description)
                .HasMaxLength(1000);

            builder.Property(h => h.Frequency)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(h => h.Priority)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(h => h.Difficulty)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(h => h.Color)
                .HasMaxLength(7);

            builder.Property(h => h.Icon)
                .HasMaxLength(50);

            builder.Property(h => h.IsArchived)
                .HasDefaultValue(false);

            builder.Property(h => h.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationship with User
            builder.HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with HabitCategory
            builder.HasOne(h => h.HabitCategory)
                .WithMany(c => c.Habits)
                .HasForeignKey(h => h.HabitCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Index
            builder.HasIndex(h => h.UserId);
            builder.HasIndex(h => h.HabitCategoryId);
        }
    }
}