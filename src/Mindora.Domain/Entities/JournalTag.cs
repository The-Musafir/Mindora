using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class JournalTag
    {
        public Guid TagId { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<JournalEntryTag> JournalEntryTags { get; set; } = new List<JournalEntryTag>();
    }
}
