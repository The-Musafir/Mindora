using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAuditController : Controller
    {
        private readonly IAdminAuditService _auditService;

        public AdminAuditController(IAdminAuditService auditService)
        {
            _auditService = auditService;
        }

        // GET: /AdminAudit
        public async Task<IActionResult> Index()
        {
            var logs = await _auditService.GetAuditLogsAsync();
            return View(logs);
        }

        // GET: /AdminAudit/LoginHistory
        public async Task<IActionResult> LoginHistory()
        {
            var history = await _auditService.GetLoginHistoriesAsync();
            return View(history);
        }
    }
}