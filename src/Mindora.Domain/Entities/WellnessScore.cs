using System;

namespace Mindora.Domain.Entities
{
    public class WellnessScore
    {
        public Guid WellnessScoreId { get; set; }
        public Guid UserId { get; set; }
        public double Score { get; set; } // 0-100
        public string? Category { get; set; } // Excellent, Good, Fair, Poor
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User User { get; set; } = null!;
    }
}