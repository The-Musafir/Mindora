using System;

namespace Mindora.Domain.Entities
{
    public class RiskPrediction
    {
        public Guid RiskPredictionId { get; set; }
        public Guid UserId { get; set; }
        public string RiskLevel { get; set; } = "Low"; // Low, Moderate, High
        public double Probability { get; set; } // 0-1
        public string? Reason { get; set; }
        public DateTime PredictedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual User User { get; set; } = null!;
    }
}