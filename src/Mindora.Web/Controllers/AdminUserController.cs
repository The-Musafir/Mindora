using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly IAdminUserManagementService _userManagementService;

        public AdminUserController(IAdminUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        // GET: /AdminUser
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var users = await _userManagementService.GetAllUsersAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View(users);
        }

        // GET: /AdminUser/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /AdminUser/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid userId, bool isActive)
        {
            await _userManagementService.ToggleUserActiveAsync(userId, isActive);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminUser/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid userId)
        {
            await _userManagementService.SoftDeleteUserAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminUser/ChangeRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(Guid userId, string roleName)
        {
            await _userManagementService.ChangeUserRoleAsync(userId, roleName);
            return RedirectToAction(nameof(Details), new { id = userId });
        }
    }
}