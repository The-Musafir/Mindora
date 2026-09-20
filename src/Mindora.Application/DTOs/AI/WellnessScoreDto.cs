namespace Mindora.Application.DTOs.AI
{
    public class WellnessScoreDto
    {
        public Guid WellnessScoreId { get; set; }
        public Guid UserId { get; set; }
        public double Score { get; set; }
        public string? Category { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}