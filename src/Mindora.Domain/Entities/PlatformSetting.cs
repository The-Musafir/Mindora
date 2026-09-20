using System;

namespace Mindora.Domain.Entities
{
    public class PlatformSetting
    {
        public Guid SettingId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}