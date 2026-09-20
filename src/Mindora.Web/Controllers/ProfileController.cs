using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Profile;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MindoraDbContext _context;
        private readonly IFileUploadService _fileUpload;
        private readonly IFollowService _followService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<User> userManager,
            MindoraDbContext context,
            IFileUploadService fileUpload,
            IFollowService followService,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _context = context;
            _fileUpload = fileUpload;
            _followService = followService;
            _logger = logger;
        }

        private Guid CurrentUserId =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                ? id
                : Guid.Empty;

        // ============================================================
        // GET: /Profile — OWN PROFILE (editable)
        // ============================================================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = await BuildProfileViewModelAsync();
            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // ============================================================
        // GET: /u/{id} — PUBLIC PROFILE (view only)
        // ============================================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Public(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
                return NotFound();

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == id);

            var roles = await _userManager.GetRolesAsync(user);

            // ============================================================
            // RECENT POSTS (non-anonymous only)
            // ============================================================
            var posts = await _context.CommunityPosts
                .AsNoTracking()
                .Where(p => p.UserId == id && !p.IsDeleted && !p.IsAnonymous)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new PublicPostItem
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    BodyPreview = p.Body.Length > 150
                        ? p.Body.Substring(0, 150) + "..."
                        : p.Body,
                    Category = p.Category,
                    CreatedAt = p.CreatedAt,
                    LikeCount = p.Reactions.Count(r => r.ReactionType == "Like"),
                    CommentCount = p.Comments.Count(c => !c.IsDeleted)
                })
                .ToListAsync();

            var totalPosts = await _context.CommunityPosts
                .CountAsync(p => p.UserId == id && !p.IsDeleted && !p.IsAnonymous);

            // ============================================================
            // RECENT REVIEWS (given by this user)
            // ============================================================
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == id)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new PublicReviewItem
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ProviderName = r.Provider.User.UserName
                        ?? r.Provider.User.Email
                        ?? "Provider"
                })
                .ToListAsync();

            var totalReviews = await _context.Reviews
                .CountAsync(r => r.UserId == id);

            double? avgRatingGiven = totalReviews > 0
                ? await _context.Reviews
                    .Where(r => r.UserId == id)
                    .AverageAsync(r => (double?)r.Rating)
                : null;

            // ============================================================
            // VIEWER CONTEXT
            // ============================================================
            var isAuthenticated = User.Identity?.IsAuthenticated == true;
            var isOwnProfile = isAuthenticated && CurrentUserId == id;

            // ============================================================
            // FOLLOW DATA
            // ============================================================
            bool isFollowing = false;
            if (isAuthenticated && !isOwnProfile)
            {
                isFollowing = await _followService.IsFollowingAsync(CurrentUserId, id);
            }

            var followerCount = await _followService.GetFollowerCountAsync(id);
            var followingCount = await _followService.GetFollowingCountAsync(id);

            // ============================================================
            // BUILD VIEW MODEL
            // ============================================================
            var viewModel = new PublicProfileViewModel
            {
                UserId = user.Id,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList(),

                DisplayName = profile?.DisplayName ?? user.UserName ?? "User",
                Bio = profile?.Bio,
                AvatarUrl = profile?.AvatarUrl,
                Gender = profile?.Gender,
                DateOfBirth = profile?.DateOfBirth,
                Timezone = profile?.Timezone ?? "UTC",

                IsOwnProfile = isOwnProfile,
                IsAuthenticated = isAuthenticated,

                // Follow system
                IsFollowing = isFollowing,
                FollowerCount = followerCount,
                FollowingCount = followingCount,

                RecentPosts = posts,
                RecentReviews = reviews,
                TotalPosts = totalPosts,
                TotalReviews = totalReviews,
                AverageRatingGiven = avgRatingGiven
            };

            return View(viewModel);
        }

        // ============================================================
        // GET: /Profile/Followers/{id}
        // ============================================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Followers(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
                return NotFound();

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == id);

            ViewBag.UserId = id;
            ViewBag.DisplayName = profile?.DisplayName ?? user.UserName ?? "User";
            ViewBag.AvatarUrl = profile?.AvatarUrl;
            ViewBag.IsOwnProfile = User.Identity?.IsAuthenticated == true && CurrentUserId == id;
            ViewBag.ActiveTab = "Followers";

            var followers = await _followService.GetFollowersAsync(id);
            return View(followers);
        }

        // ============================================================
        // GET: /Profile/Following/{id}
        // ============================================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Following(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
                return NotFound();

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == id);

            ViewBag.UserId = id;
            ViewBag.DisplayName = profile?.DisplayName ?? user.UserName ?? "User";
            ViewBag.AvatarUrl = profile?.AvatarUrl;
            ViewBag.IsOwnProfile = User.Identity?.IsAuthenticated == true && CurrentUserId == id;
            ViewBag.ActiveTab = "Following";

            var following = await _followService.GetFollowingAsync(id);
            return View(following);
        }

        // ============================================================
        // POST: /Profile/Update
        // ============================================================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                var vm = await BuildProfileViewModelAsync();
                if (vm != null)
                {
                    vm.DisplayName = request.DisplayName;
                    vm.Bio = request.Bio;
                    vm.PhoneNumber = request.PhoneNumber;
                    vm.DateOfBirth = request.DateOfBirth;
                    vm.Gender = request.Gender;
                    vm.Timezone = request.Timezone ?? "UTC";
                }
                TempData["Error"] = "Please fix the errors below.";
                return View("Index", vm);
            }

            var user = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            if (user == null) return NotFound();

            user.PhoneNumber = request.PhoneNumber;
            await _userManager.UpdateAsync(user);

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == CurrentUserId);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    ProfileId = Guid.NewGuid(),
                    UserId = CurrentUserId
                };
                _context.UserProfiles.Add(profile);
            }

            profile.DisplayName = request.DisplayName;
            profile.Bio = request.Bio;
            profile.DateOfBirth = request.DateOfBirth;
            profile.Gender = request.Gender;
            profile.Timezone = request.Timezone ?? "UTC";

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} updated profile", CurrentUserId);

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // POST: /Profile/UploadAvatar
        // ============================================================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
            {
                TempData["Error"] = "Please select an image.";
                return RedirectToAction(nameof(Index));
            }

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == CurrentUserId);

            if (profile?.AvatarUrl != null)
            {
                await _fileUpload.DeleteFileAsync(profile.AvatarUrl);
            }

            using var stream = avatar.OpenReadStream();
            var avatarUrl = await _fileUpload.UploadAvatarAsync(
                CurrentUserId,
                stream,
                avatar.FileName,
                avatar.ContentType,
                avatar.Length);

            if (avatarUrl == null)
            {
                TempData["Error"] = "Upload failed. Please try a smaller image (max 2MB, JPG/PNG/WEBP).";
                return RedirectToAction(nameof(Index));
            }

            if (profile == null)
            {
                profile = new UserProfile
                {
                    ProfileId = Guid.NewGuid(),
                    UserId = CurrentUserId,
                    DisplayName = User.Identity?.Name ?? "User"
                };
                _context.UserProfiles.Add(profile);
            }

            profile.AvatarUrl = avatarUrl;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Avatar uploaded for user {UserId}", CurrentUserId);

            TempData["Success"] = "Profile picture updated!";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // POST: /Profile/RemoveAvatar
        // ============================================================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAvatar()
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == CurrentUserId);

            if (profile?.AvatarUrl != null)
            {
                await _fileUpload.DeleteFileAsync(profile.AvatarUrl);
                profile.AvatarUrl = null;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Avatar removed for user {UserId}", CurrentUserId);
                TempData["Success"] = "Profile picture removed.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // Private helper
        // ============================================================
        private async Task<ProfileViewModel?> BuildProfileViewModelAsync()
        {
            var user = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            if (user == null) return null;

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == CurrentUserId);

            var roles = await _userManager.GetRolesAsync(user);

            return new ProfileViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList(),

                ProfileId = profile?.ProfileId,
                DisplayName = profile?.DisplayName ?? user.UserName ?? "User",
                Bio = profile?.Bio,
                DateOfBirth = profile?.DateOfBirth,
                Gender = profile?.Gender,
                AvatarUrl = profile?.AvatarUrl,
                Timezone = profile?.Timezone ?? "UTC",
                ConsentGivenAt = profile?.ConsentGivenAt
            };
        }
    }
}