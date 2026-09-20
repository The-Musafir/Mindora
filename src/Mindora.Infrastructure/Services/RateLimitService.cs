using System.Collections.Concurrent;
using Mindora.Application.Interfaces;

namespace Mindora.Infrastructure.Services
{
    /// <summary>
    /// Thread-safe in-memory sliding-window rate limiter.
    ///
    /// For multi-server scale-out, replace with Redis-backed implementation.
    /// </summary>
    public class RateLimitService : IRateLimitService
    {
        // key: "userId:action" -> queue of timestamps
        private readonly ConcurrentDictionary<string, Queue<DateTime>> _buckets = new();

        private readonly object _lock = new();

        public bool IsAllowed(string userId, string action, int maxRequests, TimeSpan window)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(action))
                return true;

            if (maxRequests <= 0) return false;

            var key = $"{userId}:{action}";
            var now = DateTime.UtcNow;
            var cutoff = now - window;

            lock (_lock)
            {
                if (!_buckets.TryGetValue(key, out var timestamps))
                {
                    timestamps = new Queue<DateTime>();
                    _buckets[key] = timestamps;
                }

                // Remove entries outside window
                while (timestamps.Count > 0 && timestamps.Peek() < cutoff)
                    timestamps.Dequeue();

                if (timestamps.Count >= maxRequests)
                    return false;

                timestamps.Enqueue(now);
                return true;
            }
        }

        public int GetRemainingRequests(string userId, string action, int maxRequests, TimeSpan window)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(action))
                return maxRequests;

            var key = $"{userId}:{action}";
            var cutoff = DateTime.UtcNow - window;

            lock (_lock)
            {
                if (!_buckets.TryGetValue(key, out var timestamps))
                    return maxRequests;

                while (timestamps.Count > 0 && timestamps.Peek() < cutoff)
                    timestamps.Dequeue();

                return Math.Max(0, maxRequests - timestamps.Count);
            }
        }

        public void Reset(string userId, string action)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(action))
                return;

            var key = $"{userId}:{action}";
            _buckets.TryRemove(key, out _);
        }

        public void CleanupExpired()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-10);

            lock (_lock)
            {
                var emptyKeys = new List<string>();

                foreach (var kvp in _buckets)
                {
                    while (kvp.Value.Count > 0 && kvp.Value.Peek() < cutoff)
                        kvp.Value.Dequeue();

                    if (kvp.Value.Count == 0)
                        emptyKeys.Add(kvp.Key);
                }

                foreach (var key in emptyKeys)
                    _buckets.TryRemove(key, out _);
            }
        }
    }
}