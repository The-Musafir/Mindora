using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;
using Mindora.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILandingPageService _landingPageService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILandingPageService landingPageService,
            ILogger<HomeController> logger)
        {
            _landingPageService = landingPageService;
            _logger = logger;
        }

        // ============================================================
        // GET: / (Landing Page)
        // ============================================================
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            Guid? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(idStr, out var id)) currentUserId = id;
            }

            var vm = await _landingPageService.BuildHomeViewModelAsync(currentUserId);
            return View(vm);
        }

        // ============================================================
        // GET: /Home/About
        // ============================================================
        [AllowAnonymous]
        public IActionResult About()
        {
            ViewData["Title"] = "About Us";
            return View();
        }

        // ============================================================
        // GET: /Home/Privacy
        // ============================================================
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy Policy";
            return View();
        }

        // ============================================================
        // GET: /Home/Terms
        // ============================================================
        [AllowAnonymous]
        public IActionResult Terms()
        {
            ViewData["Title"] = "Terms of Service";
            return View();
        }

        // ============================================================
        // GET: /Home/Contact
        // ============================================================
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact Us";
            return View();
        }

        // ============================================================
        // POST: /Home/Contact
        // ============================================================
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string subject, string message)
        {
            ViewData["Title"] = "Contact Us";

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(message))
            {
                TempData["ContactError"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(Contact));
            }

            try
            {
                _logger.LogInformation(
                    "Contact form submitted — Name: {Name}, Email: {Email}, Subject: {Subject}",
                    name, email, subject);

                // TODO: Send email notification to admin
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process contact form");
                TempData["ContactError"] = "Something went wrong. Please try again.";
                return RedirectToAction(nameof(Contact));
            }

            TempData["ContactSuccess"] = "Thank you! We'll get back to you within 24 hours.";
            return RedirectToAction(nameof(Contact));
        }

        // ============================================================
        // GET: /Home/Error
        // ============================================================
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}