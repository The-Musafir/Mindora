using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class JournalMoodConfiguration : IEntityTypeConfiguration<JournalMood>
    {
        public void Configure(EntityTypeBuilder<JournalMood> builder)
        {
            builder.HasKey(m => m.MoodId);
            builder.HasOne(m => m.Entry)
                .WithMany(j => j.Moods)
                .HasForeignKey(m => m.EntryId);
        }
    }
}
