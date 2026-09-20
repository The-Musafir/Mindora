using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportTemplateController : Controller
    {
        private readonly IReportTemplateService _templateService;

        public ReportTemplateController(IReportTemplateService templateService)
        {
            _templateService = templateService;
        }

        // GET: /ReportTemplate
        public async Task<IActionResult> Index()
        {
            var templates = await _templateService.GetAllTemplatesAsync();
            return View(templates);
        }

        // GET: /ReportTemplate/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateReportTemplateRequest());
        }

        // POST: /ReportTemplate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReportTemplateRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            try
            {
                await _templateService.CreateTemplateAsync(request);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(request);
            }
        }

        // GET: /ReportTemplate/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null) return NotFound();
            return View(template);
        }

        // POST: /ReportTemplate/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReportTemplateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _templateService.UpdateTemplateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // POST: /ReportTemplate/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid templateId, bool isActive)
        {
            await _templateService.ToggleTemplateActiveAsync(templateId, isActive);
            return RedirectToAction(nameof(Index));
        }

        // POST: /ReportTemplate/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid templateId)
        {
            await _templateService.DeleteTemplateAsync(templateId);
            return RedirectToAction(nameof(Index));
        }
    }
}