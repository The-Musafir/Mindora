using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Mindora.Web.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // লোকাল ডেভেলপমেন্টে ইমেইল পাঠানো বাদ
            return Task.CompletedTask;
        }
    }
}