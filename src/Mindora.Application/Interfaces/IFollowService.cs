using Mindora.Application.DTOs.Follow;

namespace Mindora.Application.Interfaces
{
    public interface IFollowService
    {
        
        Task<bool> FollowAsync(Guid followerId, Guid followingId);

       
        Task<bool> UnfollowAsync(Guid followerId, Guid followingId);

        
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);

       
        Task<int> GetFollowerCountAsync(Guid userId);

       
        Task<int> GetFollowingCountAsync(Guid userId);

       
        Task<IReadOnlyList<FollowUserDto>> GetFollowersAsync(Guid userId, int limit = 50);

        
        Task<IReadOnlyList<FollowUserDto>> GetFollowingAsync(Guid userId, int limit = 50);
    }
}