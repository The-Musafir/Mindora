using System;

namespace Mindora.Domain.Entities
{
    public class JournalMood
    {
        public Guid MoodId { get; set; }
        public Guid EntryId { get; set; }
        public string MoodLabel { get; set; } = string.Empty;
        public int Intensity { get; set; }
        public virtual JournalEntry Entry { get; set; } = null!;
    }
}
