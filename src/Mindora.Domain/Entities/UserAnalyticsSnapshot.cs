using System;

namespace Mindora.Domain.Entities
{
    public class UserAnalyticsSnapshot
    {
        public Guid SnapshotId { get; set; }
        public Guid UserId { get; set; }
        public string SnapshotType { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty; 
        public DateTime GeneratedAt { get; set; }
    }
}