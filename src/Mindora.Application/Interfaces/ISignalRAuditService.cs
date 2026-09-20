namespace Mindora.Application.Interfaces
{
   
    public interface ISignalRAuditService
    {
        Task LogAsync(
            Guid userId,
            string eventType,
            string message,
            string? metadata = null,
            CancellationToken cancellationToken = default);
    }
}