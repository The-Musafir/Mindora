using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.HasKey(j => j.EntryId);

            builder.Property(j => j.Title)
                .HasMaxLength(300);

            builder.Property(j => j.Content)
                .IsRequired();

            builder.Property(j => j.Category)
                .HasMaxLength(100)
                .HasDefaultValue("Personal");

            builder.Property(j => j.IsPrivate)
                .HasDefaultValue(true);

            builder.Property(j => j.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(j => j.EntryDate)
                .IsRequired();

            builder.Property(j => j.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relationship
            builder.HasOne(j => j.User)
                .WithMany()
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(j => new { j.UserId, j.EntryDate });
            builder.HasQueryFilter(j => !j.IsDeleted);
        }
    }
}