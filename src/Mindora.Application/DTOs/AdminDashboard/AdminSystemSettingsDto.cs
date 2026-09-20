namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminSystemSettingsDto
    {
        // General
        public string PlatformName { get; set; } = "Mindora";
        public string SupportEmail { get; set; } = "support@mindora.com";
        public bool MaintenanceMode { get; set; } = false;

        // Feature Toggles
        public bool HabitModuleEnabled { get; set; } = true;
        public bool JournalModuleEnabled { get; set; } = true;
        public bool AssessmentModuleEnabled { get; set; } = true;
        public bool CommunityModuleEnabled { get; set; } = true;
        public bool ProfessionalModuleEnabled { get; set; } = true;
        public bool AIModuleEnabled { get; set; } = true;

        // Payment Gateway
        public string SslcommerzStoreId { get; set; } = string.Empty;
        public string SslcommerzStorePassword { get; set; } = string.Empty;
        public bool PaymentSandboxMode { get; set; } = true;
        public bool BkashEnabled { get; set; } = true;
        public bool NagadEnabled { get; set; } = true;
        public bool CardEnabled { get; set; } = true;

        // AI Settings
        public string GeminiApiKey { get; set; } = string.Empty;
        public string DefaultAiModel { get; set; } = "gemini-1.5-flash";
        public int MaxOutputTokens { get; set; } = 512;
        public double Temperature { get; set; } = 0.7;
        public string ResponseTone { get; set; } = "Supportive"; // Supportive, Professional, Friendly

        // Email Settings
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string EmailSenderAddress { get; set; } = "no-reply@mindora.com";
        public string EmailSenderPassword { get; set; } = string.Empty;
        public bool EmailEnableSsl { get; set; } = true;

        // Security
        public int PasswordMinLength { get; set; } = 8;
        public bool PasswordRequireUppercase { get; set; } = true;
        public bool PasswordRequireLowercase { get; set; } = true;
        public bool PasswordRequireDigit { get; set; } = true;
        public bool PasswordRequireNonAlphanumeric { get; set; } = false;
        public int AccountLockoutThreshold { get; set; } = 5;
        public int SessionTimeoutMinutes { get; set; } = 60;

        // Notification
        public string DefaultNotificationChannel { get; set; } = "InApp"; // InApp, Email, Push
        public bool EmailNotificationEnabled { get; set; } = true;
        public bool PushNotificationEnabled { get; set; } = true;
        public bool InAppNotificationEnabled { get; set; } = true;
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }

        // Registration
        public bool AllowPublicRegistration { get; set; } = true;
        public bool RequireEmailConfirmation { get; set; } = false;
        public string DefaultUserRole { get; set; } = "User";
    }
}