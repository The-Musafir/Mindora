using System;

namespace Mindora.Domain.Entities
{
    public class AdminDashboardMetric
    {
        public Guid MetricId { get; set; }
        public string MetricKey { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
