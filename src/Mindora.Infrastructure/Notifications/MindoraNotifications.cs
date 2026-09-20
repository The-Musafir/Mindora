using Mindora.Application.DTOs.Notification;

namespace Mindora.Infrastructure.Notifications
{
    /// <summary>
    /// Pre-built notification factory।
    /// প্রতিটা business event এর জন্য ready-made RealtimeNotificationDto তৈরি করে।
    ///
    /// Note: Strings are in English. Localization will be handled
    /// in presentation layer (or via .resx files) in a future step.
    /// </summary>
    public static class MindoraNotifications
    {
        // ================================================================
        // HABIT MODULE
        // ================================================================

        public static RealtimeNotificationDto HabitStreakMilestone(
            string habitName, int days, Guid habitId)
        {
            return new RealtimeNotificationDto
            {
                Title = $"{days}-Day Streak!",
                Body = $"You've maintained {habitName} for {days} days. Keep going!",
                Type = "HabitMilestone",
                Channel = "InApp",
                Severity = "Success",
                Icon = "fa-fire",
                ActionUrl = "/Habit",
                ReferenceId = habitId,
                IsSoundEnabled = true
            };
        }

        public static RealtimeNotificationDto HabitRelapseAlert(
            string habitName, Guid habitId)
        {
            return new RealtimeNotificationDto
            {
                Title = "Relapse Logged",
                Body = $"You logged a relapse on {habitName}. Start again — it's part of the journey.",
                Type = "RelapseAlert",
                Channel = "InApp",
                Severity = "Warning",
                Icon = "fa-triangle-exclamation",
                ActionUrl = "/Habit",
                ReferenceId = habitId,
                IsSoundEnabled = false
            };
        }

        public static RealtimeNotificationDto HabitDailyReminder(
            string habitName, Guid habitId)
        {
            return new RealtimeNotificationDto
            {
                Title = "Daily Reminder",
                Body = $"Don't forget to track {habitName} today.",
                Type = "HabitReminder",
                Channel = "InApp",
                Severity = "Info",
                Icon = "fa-bell",
                ActionUrl = "/Habit",
                ReferenceId = habitId
            };
        }

        // ================================================================
        // COMMUNITY MODULE
        // ================================================================

        public static RealtimeNotificationDto CommunityNewComment(
            string commenterName, string postTitle, Guid postId)
        {
            return new RealtimeNotificationDto
            {
                Title = "New Comment",
                Body = $"{commenterName} commented on your post: \"{postTitle}\"",
                Type = "CommunityComment",
                Channel = "InApp",
                Severity = "Info",
                Icon = "fa-comment",
                ActionUrl = $"/Community/Post/{postId}",
                ReferenceId = postId
            };
        }

        public static RealtimeNotificationDto CommunityNewReaction(
            string reactorName, string reactionType, Guid postId)
        {
            return new RealtimeNotificationDto
            {
                Title = $"{reactionType} Reaction",
                Body = $"{reactorName} reacted to your post.",
                Type = "CommunityReaction",
                Channel = "InApp",
                Severity = "Info",
                Icon = "fa-heart",
                ActionUrl = $"/Community/Post/{postId}",
                ReferenceId = postId,
                IsSoundEnabled = false
            };
        }

        public static RealtimeNotificationDto CommunityMention(
            string mentionerName, Guid postId)
        {
            return new RealtimeNotificationDto
            {
                Title = "You Were Mentioned",
                Body = $"{mentionerName} mentioned you in a post.",
                Type = "CommunityMention",
                Channel = "InApp",
                Severity = "Info",
                Icon = "fa-at",
                ActionUrl = $"/Community/Post/{postId}",
                ReferenceId = postId
            };
        }

        // ================================================================
        // PAYMENT MODULE
        // ================================================================

        public static RealtimeNotificationDto PaymentSuccess(
            decimal amount, string planName)
        {
            return new RealtimeNotificationDto
            {
                Title = "Payment Successful",
                Body = $"Your payment of {amount:F2} was successful. {planName} subscription is now active.",
                Type = "PaymentSuccess",
                Channel = "InApp",
                Severity = "Success",
                Icon = "fa-circle-check",
                ActionUrl = "/Subscription"
            };
        }

        public static RealtimeNotificationDto PaymentFailed(decimal amount)
        {
            return new RealtimeNotificationDto
            {
                Title = "Payment Failed",
                Body = $"Your payment of {amount:F2} could not be processed. Please try again.",
                Type = "PaymentFailed",
                Channel = "InApp",
                Severity = "Error",
                Icon = "fa-circle-xmark",
                ActionUrl = "/Subscription"
            };
        }

        public static RealtimeNotificationDto SubscriptionExpiring(int daysLeft)
        {
            return new RealtimeNotificationDto
            {
                Title = "Subscription Expiring",
                Body = $"Your subscription will expire in {daysLeft} days.",
                Type = "SubscriptionExpiring",
                Channel = "InApp",
                Severity = "Warning",
                Icon = "fa-clock",
                ActionUrl = "/Subscription"
            };
        }

        // ================================================================
        // PROFESSIONAL SUPPORT MODULE
        // ================================================================

        public static RealtimeNotificationDto AppointmentConfirmed(
            string providerName, DateTime scheduledAt, Guid appointmentId)
        {
            return new RealtimeNotificationDto
            {
                Title = "Appointment Confirmed",
                Body = $"Your appointment with {providerName} is confirmed for {scheduledAt:MMM dd, HH:mm}.",
                Type = "AppointmentConfirmed",
                Channel = "InApp",
                Severity = "Success",
                Icon = "fa-calendar-check",
                ActionUrl = "/Professional/Appointments",
                ReferenceId = appointmentId
            };
        }

        public static RealtimeNotificationDto AppointmentReminder(
            string providerName, int minutesUntil, Guid appointmentId)
        {
            return new RealtimeNotificationDto
            {
                Title = "Appointment Starting Soon",
                Body = $"Your appointment with {providerName} starts in {minutesUntil} minutes.",
                Type = "AppointmentReminder",
                Channel = "InApp",
                Severity = "Warning",
                Icon = "fa-clock",
                ActionUrl = "/Professional/Appointments",
                ReferenceId = appointmentId
            };
        }

        // ================================================================
        // AI WELLNESS MODULE
        // ================================================================

        public static RealtimeNotificationDto AICrisisAlert(
            string userName, Guid sessionId)
        {
            return new RealtimeNotificationDto
            {
                Title = "Crisis Alert",
                Body = $"Crisis detected in session with user {userName}. Immediate review required.",
                Type = "AICrisisAlert",
                Channel = "InApp",
                Severity = "Crisis",
                Icon = "fa-triangle-exclamation",
                ActionUrl = "/Admin/Moderation",
                ReferenceId = sessionId,
                IsSoundEnabled = true
            };
        }

        public static RealtimeNotificationDto AIInsightReady(Guid sessionId)
        {
            return new RealtimeNotificationDto
            {
                Title = "New Insight Ready",
                Body = "A new insight has been generated from your latest session.",
                Type = "AIInsight",
                Channel = "InApp",
                Severity = "Info",
                Icon = "fa-lightbulb",
                ActionUrl = $"/AI/Session/{sessionId}",
                ReferenceId = sessionId,
                IsSoundEnabled = false
            };
        }

        // ================================================================
        // CONSULTATION MODULE
        // ================================================================

        public static RealtimeNotificationDto ConsultationStartingSoon(
            string providerName, int minutesUntil, Guid sessionId)
        {
            return new RealtimeNotificationDto
            {
                Title = "Consultation Starting Soon",
                Body = $"Your consultation with {providerName} starts in {minutesUntil} minutes.",
                Type = "ConsultationReminder",
                Channel = "InApp",
                Severity = "Warning",
                Icon = "fa-video",
                ActionUrl = $"/Consultation/Session/{sessionId}",
                ReferenceId = sessionId
            };
        }

        // ================================================================
        // ADMIN MODULE
        // ================================================================

        public static RealtimeNotificationDto AdminNewFlagReported(
            string reporterName, string reason)
        {
            return new RealtimeNotificationDto
            {
                Title = "New Flag Reported",
                Body = $"{reporterName} submitted a flag: {reason}",
                Type = "AdminNewFlag",
                Channel = "InApp",
                Severity = "Warning",
                Icon = "fa-flag",
                ActionUrl = "/AdminNotificationManagement"
            };
        }
    }
}