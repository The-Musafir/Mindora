using System;

namespace Mindora.Domain.Entities
{
    public class PlatformAnalyticsSnapshot
    {
        public Guid SnapshotId { get; set; }
        public string SnapshotType { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty; // JSON
        public DateTime GeneratedAt { get; set; }
    }
}