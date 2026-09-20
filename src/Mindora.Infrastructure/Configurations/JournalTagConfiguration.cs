using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class JournalTagConfiguration : IEntityTypeConfiguration<JournalTag>
    {
        public void Configure(EntityTypeBuilder<JournalTag> builder)
        {
            builder.HasKey(t => t.TagId);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        }
    }
}
