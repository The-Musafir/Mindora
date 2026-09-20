namespace Mindora.Application.DTOs.Analytics
{
    public class UserGrowthTrendDto
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int TotalUsers { get; set; }
    }
}