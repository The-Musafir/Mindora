using System;

namespace Mindora.Domain.Entities
{
    public class DashboardWidget
    {
        public Guid WidgetId { get; set; }
        public Guid UserId { get; set; }
        public string WidgetType { get; set; } = string.Empty;
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string? Configuration { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
