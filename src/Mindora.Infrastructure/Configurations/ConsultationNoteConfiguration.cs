using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class ConsultationNoteConfiguration : IEntityTypeConfiguration<ConsultationNote>
    {
        public void Configure(EntityTypeBuilder<ConsultationNote> builder)
        {
            builder.HasKey(n => n.NoteId);

            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(n => n.Session)
                .WithMany(s => s.NotesCollection)
                .HasForeignKey(n => n.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Provider)
                .WithMany()
                .HasForeignKey(n => n.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(n => n.SessionId);
        }
    }
}