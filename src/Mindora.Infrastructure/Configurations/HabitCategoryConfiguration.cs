using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class HabitCategoryConfiguration : IEntityTypeConfiguration<HabitCategory>
    {
        public void Configure(EntityTypeBuilder<HabitCategory> builder)
        {
            builder.HasKey(c => c.HabitCategoryId);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}