using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Community;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class CommunityController : Controller
    {
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================
        // POST
        // ============================

        // GET: /Community
        public async Task<IActionResult> Index(string? searchTerm, Guid? groupId)
        {
            var posts = await _communityService.GetPostsAsync(null, groupId, searchTerm);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.GroupId = groupId;
            ViewBag.Groups = await _communityService.GetGroupsAsync();
            return View(posts);
        }

        // GET: /Community/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var post = await _communityService.GetPostByIdAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // GET: /Community/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Groups = await _communityService.GetGroupsAsync();
            return View(new CreatePostRequest());
        }

        // POST: /Community/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = await _communityService.GetGroupsAsync();
                return View(request);
            }

            await _communityService.CreatePostAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Community/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var post = await _communityService.GetPostByIdAsync(id);
            if (post == null) return NotFound();

            var updateRequest = new UpdatePostRequest
            {
                PostId = post.PostId,
                Title = post.Title,
                Body = post.Body,
                GroupId = post.GroupId,
                Category = post.Category,
                IsAnonymous = post.IsAnonymous,
                IsPinned = post.IsPinned
            };

            ViewBag.Groups = await _communityService.GetGroupsAsync();
            return View(updateRequest);
        }

        // POST: /Community/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = await _communityService.GetGroupsAsync();
                return View(request);
            }

            await _communityService.UpdatePostAsync(request);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Community/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _communityService.DeletePostAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // COMMENT
        // ============================

        // POST: /Community/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CreateCommentRequest request)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id = request.PostId });

            await _communityService.AddCommentAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Details), new { id = request.PostId });
        }

        // POST: /Community/DeleteComment/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(Guid id, Guid postId)
        {
            await _communityService.DeleteCommentAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Details), new { id = postId });
        }

        // ============================
        // REACTION
        // ============================

        // POST: /Community/ToggleReaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReaction(Guid? postId, Guid? commentId, string reactionType)
        {
            await _communityService.ToggleReactionAsync(CurrentUserId, postId, commentId, reactionType);

            if (postId.HasValue)
                return RedirectToAction(nameof(Details), new { id = postId.Value });

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // GROUP
        // ============================

        // GET: /Community/Groups
        public async Task<IActionResult> Groups()
        {
            var groups = await _communityService.GetGroupsAsync();
            return View(groups);
        }

        // GET: /Community/Group/{id}
        public async Task<IActionResult> Group(Guid id)
        {
            var group = await _communityService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();

            var members = await _communityService.GetGroupMembersAsync(id);
            ViewBag.Members = members;
            return View(group);
        }

        // GET: /Community/CreateGroup
        public IActionResult CreateGroup() => View(new CreateGroupRequest());

        // POST: /Community/CreateGroup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            await _communityService.CreateGroupAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Groups));
        }

        // POST: /Community/JoinGroup/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinGroup(Guid id)
        {
            await _communityService.JoinGroupAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Group), new { id });
        }

        // POST: /Community/LeaveGroup/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveGroup(Guid id)
        {
            await _communityService.LeaveGroupAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Group), new { id });
        }

        // ============================
        // MODERATION
        // ============================

        // POST: /Community/Flag
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Flag(FlagRequest request)
        {
            await _communityService.FlagContentAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }
    }
}