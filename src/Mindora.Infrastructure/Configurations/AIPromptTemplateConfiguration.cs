using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class AIPromptTemplateConfiguration : IEntityTypeConfiguration<AIPromptTemplate>
    {
        public void Configure(EntityTypeBuilder<AIPromptTemplate> builder)
        {
            builder.HasKey(t => t.TemplateId);

            builder.Property(t => t.TemplateKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.PromptText)
                .IsRequired();

            builder.Property(t => t.Tone)
                .HasMaxLength(50)
                .HasDefaultValue("Supportive");

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            // Unique Key
            builder.HasIndex(t => t.TemplateKey)
                .IsUnique();
        }
    }
}