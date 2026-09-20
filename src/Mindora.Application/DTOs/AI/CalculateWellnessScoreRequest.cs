namespace Mindora.Application.DTOs.AI
{
    public class CalculateWellnessScoreRequest
    {
        public Guid UserId { get; set; }
        public double Score { get; set; } // 0-100
        public string? Category { get; set; }
    }
}