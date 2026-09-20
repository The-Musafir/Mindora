namespace Mindora.Application.Interfaces
{
    
    public interface IAIChatStreamingService
    {
       
       
        Task StreamResponseAsync(
            Guid userId,
            Guid sessionId,
            string userMessage,
            string connectionId,
            CancellationToken cancellationToken = default);
    }
}