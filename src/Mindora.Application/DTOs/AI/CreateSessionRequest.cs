namespace Mindora.Application.DTOs.AI
{
    public class CreateSessionRequest
    {
        public Guid UserId { get; set; }
        public string? InitialContext { get; set; }
    }
}