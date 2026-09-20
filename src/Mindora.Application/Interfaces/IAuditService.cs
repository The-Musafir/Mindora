namespace Mindora.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(Guid? userId, string action, string tableName, string recordId, string? oldValues = null, string? newValues = null);
    }
}