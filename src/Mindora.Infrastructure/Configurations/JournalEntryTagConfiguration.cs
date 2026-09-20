using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class JournalEntryTagConfiguration : IEntityTypeConfiguration<JournalEntryTag>
    {
        public void Configure(EntityTypeBuilder<JournalEntryTag> builder)
        {
            builder.HasKey(jt => new { jt.EntryId, jt.TagId });
            builder.HasOne(jt => jt.Entry)
                .WithMany(j => j.JournalEntryTags)
                .HasForeignKey(jt => jt.EntryId);
            builder.HasOne(jt => jt.Tag)
                .WithMany(t => t.JournalEntryTags)
                .HasForeignKey(jt => jt.TagId);
        }
    }
}
