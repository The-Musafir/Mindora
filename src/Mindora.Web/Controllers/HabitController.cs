using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Habit;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class HabitController : Controller
    {
        private readonly IHabitService _habitService;

        public HabitController(IHabitService habitService)
        {
            _habitService = habitService;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // GET: /Habit
        public async Task<IActionResult> Index()
        {
            var habits = await _habitService.GetUserHabitsAsync(CurrentUserId);
            return View(habits);
        }

        // GET: /Habit/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var habit = await _habitService.GetHabitByIdAsync(id);
            if (habit == null) return NotFound();
            return View(habit);
        }

        // GET: /Habit/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _habitService.GetAllCategoriesAsync();
            return View(new CreateHabitRequest());
        }

        // POST: /Habit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHabitRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _habitService.GetAllCategoriesAsync();
                return View(request);
            }

            await _habitService.CreateHabitAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Habit/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var habit = await _habitService.GetHabitByIdAsync(id);
            if (habit == null) return NotFound();

            var updateRequest = new UpdateHabitRequest
            {
                HabitId = habit.HabitId,
                Name = habit.Name,
                Description = habit.Description,
                CategoryId = habit.CategoryId,
                Frequency = habit.Frequency,
                Priority = habit.Priority,
                Difficulty = habit.Difficulty,
                Color = habit.Color,
                Icon = habit.Icon,
                IsArchived = habit.IsArchived
            };

            ViewBag.Categories = await _habitService.GetAllCategoriesAsync();
            return View(updateRequest);
        }

        // POST: /Habit/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateHabitRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _habitService.GetAllCategoriesAsync();
                return View(request);
            }

            await _habitService.UpdateHabitAsync(request);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Habit/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _habitService.DeleteHabitAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Habit/Statistics/{id}
        // GET: /Habit/Statistics/{id}
        public async Task<IActionResult> Statistics(Guid id)
        {
            var habit = await _habitService.GetHabitByIdAsync(id);
            if (habit == null) return NotFound();

            var stats = await _habitService.GetHabitStatisticsAsync(id);

            var vm = HabitStatisticsViewModel.Build(
                habitId: id,
                habitName: habit.Name,
                categoryName: habit.CategoryName,
                stats: stats,
                allLogs: habit.Logs ?? new List<HabitLogDto>()
            );

            return View(vm);
        }

        // GET: /Habit/Relapse/{id}
        public async Task<IActionResult> Relapse(Guid id)
        {
            var relapses = await _habitService.GetRelapsesAsync(id);
            ViewBag.HabitId = id;
            return View(relapses);
        }

        // POST: /Habit/LogRelapse/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogRelapse(Guid id, string? reason, string? note)
        {
            if (id == Guid.Empty)
                return RedirectToAction(nameof(Index));

            await _habitService.LogRelapseAsync(id, reason, note);
            return RedirectToAction(nameof(Relapse), new { id });
        }
    }
}