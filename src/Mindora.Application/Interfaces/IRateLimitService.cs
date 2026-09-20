namespace Mindora.Application.Interfaces
{
    
    public interface IRateLimitService
    {
        
        bool IsAllowed(string userId, string action, int maxRequests, TimeSpan window);

       
        int GetRemainingRequests(string userId, string action, int maxRequests, TimeSpan window);

        
        void Reset(string userId, string action);

       
        void CleanupExpired();
    }
}