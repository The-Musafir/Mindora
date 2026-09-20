using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class SereneRecoveryResource
    {
        public Guid ResourceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public virtual ICollection<SereneRecoverySession> Sessions { get; set; } = new List<SereneRecoverySession>();
    }
}
