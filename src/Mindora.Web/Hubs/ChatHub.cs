using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IRateLimitService _rateLimiter;
        private readonly ILogger<ChatHub> _logger;

        // Rate limit: 20 AI messages per minute per user
        private const int AI_MSG_MAX = 20;
        private static readonly TimeSpan AI_MSG_WINDOW = TimeSpan.FromMinutes(1);

        public ChatHub(
            IConnectionManager connectionManager,
            IRateLimitService rateLimiter,
            ILogger<ChatHub> logger)
        {
            _connectionManager = connectionManager;
            _rateLimiter = rateLimiter;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{userId}");

                _logger.LogInformation(
                    "User {UserId} connected to ChatHub (Conn: {ConnId})",
                    userId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // ============================================================
        // AI CHAT (rate limited)
        // ============================================================
        public async Task StartAIChat(Guid sessionId, string message)
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                await Clients.Caller.SendAsync("AIStreamError",
                    new { error = "Unauthorized" });
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                await Clients.Caller.SendAsync("AIStreamError",
                    new { error = "Message is required" });
                return;
            }

            // ============================================================
            // RATE LIMIT CHECK
            // ============================================================
            if (!_rateLimiter.IsAllowed(userIdStr, "ai_chat", AI_MSG_MAX, AI_MSG_WINDOW))
            {
                var remaining = _rateLimiter.GetRemainingRequests(
                    userIdStr, "ai_chat", AI_MSG_MAX, AI_MSG_WINDOW);

                _logger.LogWarning(
                    "Rate limit exceeded for user {UserId} on AI chat (Remaining: {Remaining})",
                    userIdStr, remaining);

                await Clients.Caller.SendAsync("AIStreamError", new
                {
                    error = "You're sending messages too quickly. Please wait a moment.",
                    retryAfterSeconds = (int)AI_MSG_WINDOW.TotalSeconds,
                    rateLimited = true
                });
                return;
            }

            var streamingService = Context.GetHttpContext()?
                .RequestServices.GetService<IAIChatStreamingService>();

            if (streamingService == null)
            {
                await Clients.Caller.SendAsync("AIStreamError",
                    new { error = "Service unavailable" });
                return;
            }

            try
            {
                await streamingService.StreamResponseAsync(
                    userId, sessionId, message, Context.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StartAIChat failed");
                await Clients.Caller.SendAsync("AIStreamError",
                    new { error = "Something went wrong" });
            }
        }

        // ============================================================
        // TYPING INDICATOR
        // ============================================================
        public async Task SendTypingIndicator(string targetUserId, bool isTyping)
        {
            var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId)) return;

            await Clients.Group($"chat-{targetUserId}")
                .SendAsync("TypingIndicator", senderId, isTyping);
        }
    }
}