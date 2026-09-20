namespace Mindora.Application.Interfaces
{
    public interface IConnectionManager
    {
        /// <summary>
        /// Add a connection for a user.
        /// </summary>
        void AddConnection(string userId, string connectionId);

        /// <summary>
        /// Remove a connection for a user.
        /// </summary>
        void RemoveConnection(string userId, string connectionId);

        /// <summary>
        /// Get all active connection IDs for a user.
        /// </summary>
        IReadOnlyList<string> GetConnections(string userId);

        /// <summary>
        /// Check if a user is currently online.
        /// </summary>
        bool IsOnline(string userId);

        /// <summary>
        /// Get total number of online users.
        /// </summary>
        int GetOnlineUserCount();

        /// <summary>
        /// Get all online user IDs.
        /// </summary>
        IReadOnlyList<string> GetOnlineUserIds();

        /// <summary>
        /// Remove all connections for a user (used on logout/cleanup).
        /// </summary>
        void RemoveAllConnections(string userId);
    }
}