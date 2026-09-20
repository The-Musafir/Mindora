namespace Mindora.Application.DTOs.AI
{
    public class CreateRiskPredictionRequest
    {
        public Guid UserId { get; set; }
        public string RiskLevel { get; set; } = "Low";
        public double Probability { get; set; }
        public string? Reason { get; set; }
    }
}