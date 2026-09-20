namespace Mindora.Application.DTOs.Habit
{
    public class RelapseLogDto
    {
        public Guid RelapseLogId { get; set; }
        public DateTime RelapseDate { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
    }
}