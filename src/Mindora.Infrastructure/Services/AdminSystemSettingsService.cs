using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminSystemSettingsService : IAdminSystemSettingsService
    {
        private readonly MindoraDbContext _context;

        public AdminSystemSettingsService(MindoraDbContext context)
        {
            _context = context;
        }

        // ============================
        // GET SETTINGS
        // ============================
        public async Task<AdminSystemSettingsDto> GetSettingsAsync()
        {
            var settings = await _context.PlatformSettings.ToListAsync();

            var dto = new AdminSystemSettingsDto();

            foreach (var setting in settings)
            {
                switch (setting.Key)
                {
                    // General
                    case "PlatformName": dto.PlatformName = setting.Value; break;
                    case "SupportEmail": dto.SupportEmail = setting.Value; break;
                    case "MaintenanceMode": dto.MaintenanceMode = setting.Value == "true"; break;

                    // Feature Toggles
                    case "HabitModuleEnabled": dto.HabitModuleEnabled = setting.Value == "true"; break;
                    case "JournalModuleEnabled": dto.JournalModuleEnabled = setting.Value == "true"; break;
                    case "AssessmentModuleEnabled": dto.AssessmentModuleEnabled = setting.Value == "true"; break;
                    case "CommunityModuleEnabled": dto.CommunityModuleEnabled = setting.Value == "true"; break;
                    case "ProfessionalModuleEnabled": dto.ProfessionalModuleEnabled = setting.Value == "true"; break;
                    case "AIModuleEnabled": dto.AIModuleEnabled = setting.Value == "true"; break;

                    // Payment Gateway
                    case "SslcommerzStoreId": dto.SslcommerzStoreId = setting.Value; break;
                    case "SslcommerzStorePassword": dto.SslcommerzStorePassword = setting.Value; break;
                    case "PaymentSandboxMode": dto.PaymentSandboxMode = setting.Value == "true"; break;
                    case "BkashEnabled": dto.BkashEnabled = setting.Value == "true"; break;
                    case "NagadEnabled": dto.NagadEnabled = setting.Value == "true"; break;
                    case "CardEnabled": dto.CardEnabled = setting.Value == "true"; break;

                    // AI Settings
                    case "GeminiApiKey": dto.GeminiApiKey = setting.Value; break;
                    case "DefaultAiModel": dto.DefaultAiModel = setting.Value; break;
                    case "MaxOutputTokens":
                        if (int.TryParse(setting.Value, out int maxToken))
                            dto.MaxOutputTokens = maxToken;
                        break;
                    case "Temperature":
                        if (double.TryParse(setting.Value, out double temp))
                            dto.Temperature = temp;
                        break;
                    case "ResponseTone": dto.ResponseTone = setting.Value; break;

                    // Email Settings
                    case "SmtpHost": dto.SmtpHost = setting.Value; break;
                    case "SmtpPort":
                        if (int.TryParse(setting.Value, out int port))
                            dto.SmtpPort = port;
                        break;
                    case "EmailSenderAddress": dto.EmailSenderAddress = setting.Value; break;
                    case "EmailSenderPassword": dto.EmailSenderPassword = setting.Value; break;
                    case "EmailEnableSsl": dto.EmailEnableSsl = setting.Value == "true"; break;

                    // Security
                    case "PasswordMinLength":
                        if (int.TryParse(setting.Value, out int minLen))
                            dto.PasswordMinLength = minLen;
                        break;
                    case "PasswordRequireUppercase": dto.PasswordRequireUppercase = setting.Value == "true"; break;
                    case "PasswordRequireLowercase": dto.PasswordRequireLowercase = setting.Value == "true"; break;
                    case "PasswordRequireDigit": dto.PasswordRequireDigit = setting.Value == "true"; break;
                    case "PasswordRequireNonAlphanumeric": dto.PasswordRequireNonAlphanumeric = setting.Value == "true"; break;
                    case "AccountLockoutThreshold":
                        if (int.TryParse(setting.Value, out int lockout))
                            dto.AccountLockoutThreshold = lockout;
                        break;
                    case "SessionTimeoutMinutes":
                        if (int.TryParse(setting.Value, out int sessionTimeout))
                            dto.SessionTimeoutMinutes = sessionTimeout;
                        break;

                    // Notification Defaults
                    case "DefaultNotificationChannel": dto.DefaultNotificationChannel = setting.Value; break;
                    case "EmailNotificationEnabled": dto.EmailNotificationEnabled = setting.Value == "true"; break;
                    case "PushNotificationEnabled": dto.PushNotificationEnabled = setting.Value == "true"; break;
                    case "InAppNotificationEnabled": dto.InAppNotificationEnabled = setting.Value == "true"; break;
                    case "QuietHoursStart":
                        if (TimeSpan.TryParse(setting.Value, out TimeSpan qhs))
                            dto.QuietHoursStart = qhs;
                        break;
                    case "QuietHoursEnd":
                        if (TimeSpan.TryParse(setting.Value, out TimeSpan qhe))
                            dto.QuietHoursEnd = qhe;
                        break;

                    // Registration
                    case "AllowPublicRegistration": dto.AllowPublicRegistration = setting.Value == "true"; break;
                    case "RequireEmailConfirmation": dto.RequireEmailConfirmation = setting.Value == "true"; break;
                    case "DefaultUserRole": dto.DefaultUserRole = setting.Value; break;
                }
            }

            return dto;
        }

        // ============================
        // UPDATE SETTINGS
        // ============================
        public async Task<bool> UpdateSettingsAsync(AdminSystemSettingsDto dto)
        {
            await SetSettingAsync("PlatformName", dto.PlatformName);
            await SetSettingAsync("SupportEmail", dto.SupportEmail);
            await SetSettingAsync("MaintenanceMode", dto.MaintenanceMode.ToString().ToLower());

            await SetSettingAsync("HabitModuleEnabled", dto.HabitModuleEnabled.ToString().ToLower());
            await SetSettingAsync("JournalModuleEnabled", dto.JournalModuleEnabled.ToString().ToLower());
            await SetSettingAsync("AssessmentModuleEnabled", dto.AssessmentModuleEnabled.ToString().ToLower());
            await SetSettingAsync("CommunityModuleEnabled", dto.CommunityModuleEnabled.ToString().ToLower());
            await SetSettingAsync("ProfessionalModuleEnabled", dto.ProfessionalModuleEnabled.ToString().ToLower());
            await SetSettingAsync("AIModuleEnabled", dto.AIModuleEnabled.ToString().ToLower());

            await SetSettingAsync("SslcommerzStoreId", dto.SslcommerzStoreId);
            await SetSettingAsync("SslcommerzStorePassword", dto.SslcommerzStorePassword);
            await SetSettingAsync("PaymentSandboxMode", dto.PaymentSandboxMode.ToString().ToLower());
            await SetSettingAsync("BkashEnabled", dto.BkashEnabled.ToString().ToLower());
            await SetSettingAsync("NagadEnabled", dto.NagadEnabled.ToString().ToLower());
            await SetSettingAsync("CardEnabled", dto.CardEnabled.ToString().ToLower());

            await SetSettingAsync("GeminiApiKey", dto.GeminiApiKey);
            await SetSettingAsync("DefaultAiModel", dto.DefaultAiModel);
            await SetSettingAsync("MaxOutputTokens", dto.MaxOutputTokens.ToString());
            await SetSettingAsync("Temperature", dto.Temperature.ToString("F2"));
            await SetSettingAsync("ResponseTone", dto.ResponseTone);

            await SetSettingAsync("SmtpHost", dto.SmtpHost);
            await SetSettingAsync("SmtpPort", dto.SmtpPort.ToString());
            await SetSettingAsync("EmailSenderAddress", dto.EmailSenderAddress);
            await SetSettingAsync("EmailSenderPassword", dto.EmailSenderPassword);
            await SetSettingAsync("EmailEnableSsl", dto.EmailEnableSsl.ToString().ToLower());

            await SetSettingAsync("PasswordMinLength", dto.PasswordMinLength.ToString());
            await SetSettingAsync("PasswordRequireUppercase", dto.PasswordRequireUppercase.ToString().ToLower());
            await SetSettingAsync("PasswordRequireLowercase", dto.PasswordRequireLowercase.ToString().ToLower());
            await SetSettingAsync("PasswordRequireDigit", dto.PasswordRequireDigit.ToString().ToLower());
            await SetSettingAsync("PasswordRequireNonAlphanumeric", dto.PasswordRequireNonAlphanumeric.ToString().ToLower());
            await SetSettingAsync("AccountLockoutThreshold", dto.AccountLockoutThreshold.ToString());
            await SetSettingAsync("SessionTimeoutMinutes", dto.SessionTimeoutMinutes.ToString());

            await SetSettingAsync("DefaultNotificationChannel", dto.DefaultNotificationChannel);
            await SetSettingAsync("EmailNotificationEnabled", dto.EmailNotificationEnabled.ToString().ToLower());
            await SetSettingAsync("PushNotificationEnabled", dto.PushNotificationEnabled.ToString().ToLower());
            await SetSettingAsync("InAppNotificationEnabled", dto.InAppNotificationEnabled.ToString().ToLower());
            await SetSettingAsync("QuietHoursStart", dto.QuietHoursStart?.ToString(@"hh\:mm") ?? "");
            await SetSettingAsync("QuietHoursEnd", dto.QuietHoursEnd?.ToString(@"hh\:mm") ?? "");

            await SetSettingAsync("AllowPublicRegistration", dto.AllowPublicRegistration.ToString().ToLower());
            await SetSettingAsync("RequireEmailConfirmation", dto.RequireEmailConfirmation.ToString().ToLower());
            await SetSettingAsync("DefaultUserRole", dto.DefaultUserRole);

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // PRIVATE HELPER
        // ============================
        private async Task SetSettingAsync(string key, string value)
        {
            // ✅ null guard: কখনো Value NULL হবে না
            value ??= string.Empty;

            var existing = await _context.PlatformSettings
                .FirstOrDefaultAsync(s => s.Key == key);

            if (existing == null)
            {
                _context.PlatformSettings.Add(new PlatformSetting
                {
                    SettingId = Guid.NewGuid(),
                    Key = key,
                    Value = value,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.Value = value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }
        
    }
}