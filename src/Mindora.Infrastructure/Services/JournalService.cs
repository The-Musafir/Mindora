using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Journal;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class JournalService : IJournalService
    {
        private readonly MindoraDbContext _context;

        public JournalService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<JournalResponse> CreateJournalAsync(Guid userId, CreateJournalRequest request)
        {
            var entry = new JournalEntry
            {
                EntryId = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Content = request.Content,
                IsPrivate = request.IsPrivate,
                Category = request.Category,
                EntryDate = request.EntryDate,
                CreatedAt = DateTime.UtcNow
            };

            // Mood
            if (!string.IsNullOrWhiteSpace(request.MoodLabel))
            {
                entry.Moods.Add(new JournalMood
                {
                    MoodId = Guid.NewGuid(),
                    EntryId = entry.EntryId,
                    MoodLabel = request.MoodLabel,
                    Intensity = request.MoodIntensity ?? 3
                });
            }

            // Tags
            foreach (var tagName in request.Tags.Distinct())
            {
                var existingTag = await _context.JournalTags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (existingTag == null)
                {
                    existingTag = new JournalTag { TagId = Guid.NewGuid(), Name = tagName };
                    _context.JournalTags.Add(existingTag);
                }
                entry.JournalEntryTags.Add(new JournalEntryTag { EntryId = entry.EntryId, TagId = existingTag.TagId });
            }

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
            return await MapToResponseAsync(entry);
        }

        public async Task<JournalResponse?> GetJournalByIdAsync(Guid entryId)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Moods)
                .Include(j => j.JournalEntryTags).ThenInclude(jt => jt.Tag)
                .FirstOrDefaultAsync(j => j.EntryId == entryId && !j.IsDeleted);

            return entry == null ? null : await MapToResponseAsync(entry);
        }

        public async Task<IReadOnlyList<JournalResponse>> GetUserJournalsAsync(Guid userId, string? searchTerm = null, string? category = null)
        {
            var query = _context.JournalEntries
                .Where(j => j.UserId == userId && !j.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(j => j.Title.Contains(searchTerm) || j.Content.Contains(searchTerm));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(j => j.Category == category);

            var entries = await query
                .Include(j => j.Moods)
                .Include(j => j.JournalEntryTags).ThenInclude(jt => jt.Tag)
                .OrderByDescending(j => j.EntryDate)
                .ToListAsync();

            var result = new List<JournalResponse>();
            foreach (var entry in entries)
                result.Add(await MapToResponseAsync(entry));

            return result;
        }

        public async Task<JournalResponse> UpdateJournalAsync(UpdateJournalRequest request)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Moods)
                .Include(j => j.JournalEntryTags).ThenInclude(jt => jt.Tag)
                .FirstOrDefaultAsync(j => j.EntryId == request.EntryId && !j.IsDeleted);

            if (entry == null) throw new KeyNotFoundException($"Journal entry {request.EntryId} not found.");

            entry.Title = request.Title;
            entry.Content = request.Content;
            entry.IsPrivate = request.IsPrivate;
            entry.Category = request.Category;
            entry.EntryDate = request.EntryDate;
            entry.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await MapToResponseAsync(entry);
        }

        public async Task<bool> DeleteJournalAsync(Guid entryId)
        {
            var entry = await _context.JournalEntries.FindAsync(entryId);
            if (entry == null || entry.IsDeleted) return false;

            entry.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JournalStatisticsDto> GetJournalStatisticsAsync(Guid userId)
        {
            var entries = await _context.JournalEntries
                .Where(j => j.UserId == userId && !j.IsDeleted)
                .ToListAsync();

            var thisWeek = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-7);
            var thisMonth = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1);

            var moodCounts = new Dictionary<string, int>();
            var categoryCounts = new Dictionary<string, int>();

            foreach (var entry in entries)
            {
                var moods = await _context.JournalMoods.Where(m => m.EntryId == entry.EntryId).ToListAsync();
                foreach (var mood in moods)
                {
                    if (!moodCounts.ContainsKey(mood.MoodLabel))
                        moodCounts[mood.MoodLabel] = 0;
                    moodCounts[mood.MoodLabel]++;
                }

                if (!categoryCounts.ContainsKey(entry.Category))
                    categoryCounts[entry.Category] = 0;
                categoryCounts[entry.Category]++;
            }

            return new JournalStatisticsDto
            {
                TotalEntries = entries.Count,
                PrivateEntries = entries.Count(e => e.IsPrivate),
                PublicEntries = entries.Count(e => !e.IsPrivate),
                ThisWeekEntries = entries.Count(e => e.EntryDate >= thisWeek),
                ThisMonthEntries = entries.Count(e => e.EntryDate >= thisMonth),
                MoodCounts = moodCounts,
                CategoryCounts = categoryCounts
            };
        }

        private async Task<JournalResponse> MapToResponseAsync(JournalEntry entry)
        {
            return new JournalResponse
            {
                EntryId = entry.EntryId,
                Title = entry.Title,
                Content = entry.Content,
                IsPrivate = entry.IsPrivate,
                Category = entry.Category,
                EntryDate = entry.EntryDate,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,
                Tags = entry.JournalEntryTags.Select(jt => jt.Tag.Name).ToList(),
                Moods = entry.Moods.Select(m => new JournalMoodDto
                {
                    MoodId = m.MoodId,
                    MoodLabel = m.MoodLabel,
                    Intensity = m.Intensity
                }).ToList()
            };
        }
    }
}