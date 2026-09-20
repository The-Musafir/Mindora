namespace Mindora.Domain.Entities
{
    public class RelapseLog
    {
        public Guid RelapseLogId { get; set; }
        public Guid HabitId { get; set; }
        public DateTime RelapseDate { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }

        public virtual Habit Habit { get; set; } = null!;
    }
}