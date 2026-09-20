using Mindora.Application.DTOs.Community;

namespace Mindora.Application.Interfaces
{
    public interface ICommunityService
    {
        // Post
        Task<PostResponse> CreatePostAsync(Guid userId, CreatePostRequest request);
        Task<PostResponse?> GetPostByIdAsync(Guid postId);
        Task<IReadOnlyList<PostResponse>> GetPostsAsync(Guid? userId = null, Guid? groupId = null, string? searchTerm = null);
        Task<PostResponse> UpdatePostAsync(UpdatePostRequest request);
        Task<bool> DeletePostAsync(Guid postId, Guid userId);
        Task<bool> PinPostAsync(Guid postId, bool isPinned);

        // Comment
        Task<CommentDto> AddCommentAsync(Guid userId, CreateCommentRequest request);
        Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);

        // Reaction
        Task<bool> ToggleReactionAsync(Guid userId, Guid? postId, Guid? commentId, string reactionType);

        // Group
        Task<GroupResponse> CreateGroupAsync(Guid userId, CreateGroupRequest request);
        Task<GroupResponse?> GetGroupByIdAsync(Guid groupId);
        Task<IReadOnlyList<GroupResponse>> GetGroupsAsync();
        Task<bool> JoinGroupAsync(Guid groupId, Guid userId);
        Task<bool> LeaveGroupAsync(Guid groupId, Guid userId);
        Task<IReadOnlyList<GroupMemberDto>> GetGroupMembersAsync(Guid groupId);

        // Moderation
        Task<Guid> FlagContentAsync(Guid userId, FlagRequest request);
        Task<IReadOnlyList<ModerationFlagDto>> GetPendingFlagsAsync();
        Task<bool> ResolveFlagAsync(Guid flagId, string resolution, Guid adminUserId);
    }
}