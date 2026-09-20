using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class JournalEntry
    {
        public Guid EntryId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = true;
        public string Category { get; set; } = "Personal"; // নতুন
        public DateOnly EntryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual ICollection<JournalMood> Moods { get; set; } = new List<JournalMood>();
        public virtual ICollection<JournalEntryTag> JournalEntryTags { get; set; } = new List<JournalEntryTag>();
    }
}