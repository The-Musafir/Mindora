using System.Collections.Concurrent;
using Mindora.Application.Interfaces;

namespace Mindora.Infrastructure.Services
{
    /// <summary>
    /// Thread-safe in-memory connection manager for SignalR.
    /// Tracks UserId → List of ConnectionIds.
    /// For multi-server scale-out, replace with Redis-backed store.
    /// </summary>
    public class ConnectionManager : IConnectionManager
    {
        // userId -> list of connectionIds
        private readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

        // connectionId -> userId (for reverse lookup)
        private readonly ConcurrentDictionary<string, string> _connectionToUser = new();

        private readonly object _lock = new();

        public void AddConnection(string userId, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(connectionId))
                return;

            lock (_lock)
            {
                if (!_userConnections.ContainsKey(userId))
                    _userConnections[userId] = new HashSet<string>();

                _userConnections[userId].Add(connectionId);
                _connectionToUser[connectionId] = userId;
            }
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(connectionId))
                return;

            lock (_lock)
            {
                if (_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId].Remove(connectionId);
                    if (_userConnections[userId].Count == 0)
                    {
                        _userConnections.TryRemove(userId, out _);
                    }
                }

                _connectionToUser.TryRemove(connectionId, out _);
            }
        }

        public IReadOnlyList<string> GetConnections(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Array.Empty<string>();

            if (_userConnections.TryGetValue(userId, out var connections))
            {
                lock (_lock)
                {
                    return connections.ToList();
                }
            }

            return Array.Empty<string>();
        }

        public bool IsOnline(string userId)
        {
            return !string.IsNullOrWhiteSpace(userId) &&
                   _userConnections.ContainsKey(userId);
        }

        public int GetOnlineUserCount()
        {
            return _userConnections.Count;
        }

        public IReadOnlyList<string> GetOnlineUserIds()
        {
            return _userConnections.Keys.ToList();
        }

        public void RemoveAllConnections(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;

            lock (_lock)
            {
                if (_userConnections.TryRemove(userId, out var connections))
                {
                    foreach (var connectionId in connections)
                    {
                        _connectionToUser.TryRemove(connectionId, out _);
                    }
                }
            }
        }
    }
}