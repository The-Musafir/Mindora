namespace Mindora.Application.DTOs.AI
{
    public class AIWellnessCoachSessionDto
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsActive { get; set; }
        public string? LastContext { get; set; }
        public int MessageCount { get; set; }
        public List<AIChatMessageDto> Messages { get; set; } = new();
    }
}