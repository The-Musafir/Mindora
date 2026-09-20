using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class BoredomRecoveryActivity
    {
        public Guid ActivityId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<BoredomRecoverySession> Sessions { get; set; } = new List<BoredomRecoverySession>();
    }
}
