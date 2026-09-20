using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Analytics;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AnalyticsEventService : IAnalyticsEventService
    {
        private readonly MindoraDbContext _context;

        public AnalyticsEventService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task LogEventAsync(CreateAnalyticsEventDto dto)
        {
            var evt = new PlatformAnalyticsEvent
            {
                UserId = dto.UserId,
                EventType = dto.EventType,
                Payload = dto.Payload,
                Timestamp = DateTime.UtcNow
            };

            _context.PlatformAnalyticsEvents.Add(evt);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<AnalyticsEventDto>> GetRecentEventsAsync(int count = 100)
        {
            var events = await _context.PlatformAnalyticsEvents
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToListAsync();

            return events.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<AnalyticsEventDto>> GetEventsByUserAsync(Guid userId, int count = 100)
        {
            var events = await _context.PlatformAnalyticsEvents
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToListAsync();

            return events.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<AnalyticsEventDto>> GetEventsByTypeAsync(string eventType, int count = 100)
        {
            var events = await _context.PlatformAnalyticsEvents
                .Where(e => e.EventType == eventType)
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToListAsync();

            return events.Select(MapToDto).ToList();
        }

        private AnalyticsEventDto MapToDto(PlatformAnalyticsEvent evt)
        {
            return new AnalyticsEventDto
            {
                EventId = evt.EventId,
                UserId = evt.UserId,
                EventType = evt.EventType,
                Payload = evt.Payload,
                Timestamp = evt.Timestamp
            };
        }
    }
}