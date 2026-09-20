namespace Mindora.Web.Realtime
{
    
    public static class HubEvents
    {
        // Notification
        public const string ReceiveNotification = "ReceiveNotification";
        public const string UnreadCountUpdated = "UnreadCountUpdated";
        public const string NotificationDismissed = "NotificationDismissed";

        // Chat (Step 9)
        public const string MessageReceived = "MessageReceived";
        public const string UserTyping = "UserTyping";
        public const string MessageRead = "MessageRead";

        // Presence (Step 8)
        public const string UserOnline = "UserOnline";
        public const string UserOffline = "UserOffline";

        // Admin (Step 5)
        public const string CrisisAlert = "CrisisAlert";
        public const string NewFlagReported = "NewFlagReported";
    }
}