namespace Mindora.Application.DTOs.Consultation
{
    public class CreateFollowUpPlanRequest
    {
        public Guid SessionId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}