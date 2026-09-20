using System;

namespace Mindora.Domain.Entities
{
    public class DataPrivacyRequest
    {
        public Guid RequestId { get; set; }
        public Guid UserId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
