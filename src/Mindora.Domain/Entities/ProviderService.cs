using System;

namespace Mindora.Domain.Entities
{
    public class ProviderService
    {
        public Guid ServiceId { get; set; }
        public Guid PracticeId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal? Fee { get; set; }

        // Navigation Properties
        public virtual ProviderPracticeDetail Practice { get; set; } = null!;
    }
}