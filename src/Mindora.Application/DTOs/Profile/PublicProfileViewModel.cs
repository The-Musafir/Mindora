namespace Mindora.Application.DTOs.Profile
{
   
    public class PublicProfileViewModel
    {
        // ============================================================
        // USER INFO
        // ============================================================
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }

       
        public DateTime MemberSince { get; set; }

        /// <summary>
        /// Alias for MemberSince (used in controller).
        /// </summary>
        public DateTime CreatedAt
        {
            get => MemberSince;
            set => MemberSince = value;
        }

        // Optional extra profile fields
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Timezone { get; set; } = "UTC";

        public IList<string> Roles { get; set; } = new List<string>();

        // ============================================================
        // VIEWER CONTEXT
        // ============================================================
        /// <summary>Is this the current viewer's own profile?</summary>
        public bool IsOwnProfile { get; set; }

        /// <summary>Is the current viewer authenticated?</summary>
        public bool IsAuthenticated { get; set; }

        // ============================================================
        // FOLLOW SYSTEM
        // ============================================================
        public bool IsFollowing { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }

        // ============================================================
        // PUBLIC ACTIVITY
        // ============================================================
        public List<PublicPostItem> RecentPosts { get; set; } = new();
        public List<PublicReviewItem> RecentReviews { get; set; } = new();

        public int TotalPosts { get; set; }
        public int TotalReviews { get; set; }
        public double? AverageRatingGiven { get; set; }

        // ============================================================
        // COMPUTED
        // ============================================================
        public string PrimaryRole =>
            Roles.Contains("Admin") ? "Admin" :
            Roles.Contains("Professional") ? "Professional" :
            Roles.Contains("Moderator") ? "Moderator" :
            "Member";

        public string RoleBadgeColor =>
            PrimaryRole switch
            {
                "Admin" => "#EF4444",
                "Professional" => "#8B5CF6",
                "Moderator" => "#F59E0B",
                _ => "#06B6D4"
            };

        public string Initial =>
            !string.IsNullOrEmpty(DisplayName)
                ? DisplayName.Substring(0, 1).ToUpper()
                : "U";

        public bool HasActivity =>
            RecentPosts.Any() || RecentReviews.Any();
    }

    
    public class PublicPostItem
    {
        public Guid PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BodyPreview { get; set; } = string.Empty;
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }

    /// <summary>
    /// Public review written by this user.
    /// </summary>
    public class PublicReviewItem
    {
        public Guid ReviewId { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}