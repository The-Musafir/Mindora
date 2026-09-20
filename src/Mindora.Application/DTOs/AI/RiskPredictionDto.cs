namespace Mindora.Application.DTOs.AI
{
    public class RiskPredictionDto
    {
        public Guid RiskPredictionId { get; set; }
        public Guid UserId { get; set; }
        public string RiskLevel { get; set; } = "Low";
        public double Probability { get; set; }
        public string? Reason { get; set; }
        public DateTime PredictedAt { get; set; }
    }
}