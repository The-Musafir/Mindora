using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Journal;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class JournalController : Controller
    {
        private readonly IJournalService _journalService;

        public JournalController(IJournalService journalService)
        {
            _journalService = journalService;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================================================
        // GET: /Journal
        // ============================================================
        public async Task<IActionResult> Index(string? searchTerm, string? category)
        {
            var journals = await _journalService.GetUserJournalsAsync(CurrentUserId, searchTerm, category);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Category = category;
            return View(journals);
        }

        // ============================================================
        // CREATE
        // ============================================================
        [HttpGet]
        public IActionResult Create() => View(new CreateJournalRequest());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJournalRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            await _journalService.CreateJournalAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // DETAILS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var entry = await _journalService.GetJournalByIdAsync(id);
            if (entry == null) return NotFound();
            return View(entry);
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entry = await _journalService.GetJournalByIdAsync(id);
            if (entry == null) return NotFound();

            var updateRequest = new UpdateJournalRequest
            {
                EntryId = entry.EntryId,
                Title = entry.Title,
                Content = entry.Content,
                IsPrivate = entry.IsPrivate,
                Category = entry.Category,
                EntryDate = entry.EntryDate
            };
            return View(updateRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateJournalRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            await _journalService.UpdateJournalAsync(request);
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _journalService.DeleteJournalAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // GET: /Journal/Statistics
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

            var entries = await _journalService.GetUserJournalsAsync(userId)
                          ?? new List<JournalResponse>();

            var today = DateTime.UtcNow.Date;
            var thisMonthStart = new DateTime(today.Year, today.Month, 1);

            // ============================================================
            // BASE STATS
            // ============================================================
            var entriesList = entries.ToList();
            var entriesWithMood = entriesList
                .Where(e => e.Moods != null && e.Moods.Any())
                .ToList();

            var vm = new JournalStatisticsViewModel
            {
                TotalEntries = entriesList.Count,
                EntriesThisMonth = entriesList.Count(e => e.CreatedAt >= thisMonthStart),
                AverageMood = entriesWithMood.Any()
                    ? entriesWithMood.Average(e => e.Moods.Count)
                    : 0,
                CurrentStreak = CalculateJournalStreak(entriesList, today),
                LongestStreak = CalculateLongestStreak(entriesList)
            };

            // ============================================================
            // MOST COMMON CATEGORY
            // ============================================================
            var categoryGroups = entriesList
                .Where(e => !string.IsNullOrEmpty(e.Category))
                .GroupBy(e => e.Category)
                .OrderByDescending(g => g.Count())
                .ToList();

            if (categoryGroups.Any())
            {
                vm.MostCommonMood = categoryGroups.First().Key;
            }

            // ============================================================
            // MOOD TIMELINE (last 30 days)
            // ============================================================
            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var dayEntry = entriesList.FirstOrDefault(e => e.CreatedAt.Date == date);

                vm.MoodTimeline.Add(new MoodTimelinePoint
                {
                    Date = date,
                    MoodScore = dayEntry?.Moods?.Count ?? 0
                });
            }

            // ============================================================
            // MOOD DISTRIBUTION (by category)
            // ============================================================
            var moodColors = new[] { "#06B6D4", "#8B5CF6", "#F59E0B", "#10B981", "#EC4899", "#3B82F6" };
            var moodIndex = 0;

            foreach (var group in categoryGroups.Take(6))
            {
                vm.MoodDistribution.Add(new MoodDistributionItem
                {
                    MoodName = group.Key,
                    Count = group.Count(),
                    Color = moodColors[moodIndex % moodColors.Length]
                });
                moodIndex++;
            }

            // ============================================================
            // WRITING HEATMAP (last 30 days)
            // ============================================================
            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                vm.HeatmapDays.Add(new WritingHeatmapDay
                {
                    Date = date,
                    Wrote = entriesList.Any(e => e.CreatedAt.Date == date)
                });
            }

            return View(vm);
        }

        // ============================================================
        // HELPER METHODS
        // ============================================================

        // Current writing streak
        private static int CalculateJournalStreak(
            List<JournalResponse> entries,
            DateTime today)
        {
            int streak = 0;
            for (int i = 0; i < 365; i++)
            {
                var date = today.AddDays(-i);
                if (entries.Any(e => e.CreatedAt.Date == date))
                    streak++;
                else if (i > 0)
                    break;
            }
            return streak;
        }

        // Longest writing streak
        private static int CalculateLongestStreak(List<JournalResponse> entries)
        {
            if (!entries.Any()) return 0;

            var dates = entries
                .Select(e => e.CreatedAt.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            int longest = 1, current = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i] == dates[i - 1].AddDays(1))
                {
                    current++;
                    longest = Math.Max(longest, current);
                }
                else
                {
                    current = 1;
                }
            }

            return longest;
        }
    }
}