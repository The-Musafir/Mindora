using System;

namespace Mindora.Domain.Entities
{
    public class JournalEntryTag
    {
        public Guid EntryId { get; set; }
        public Guid TagId { get; set; }
        public virtual JournalEntry Entry { get; set; } = null!;
        public virtual JournalTag Tag { get; set; } = null!;
    }
}
