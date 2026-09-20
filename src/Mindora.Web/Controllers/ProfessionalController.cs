using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Professional;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class ProfessionalController : Controller
    {
        private readonly IProfessionalService _professionalService;

        public ProfessionalController(IProfessionalService professionalService)
        {
            _professionalService = professionalService;
        }

        private Guid CurrentUserId => Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : Guid.Empty;

        // ============================
        // PROVIDER DIRECTORY
        // ============================

        // GET: /Professional
        public async Task<IActionResult> Index(string? searchTerm, Guid? specialtyId)
        {
            var providers = await _professionalService.GetAllProvidersAsync(searchTerm, specialtyId);
            ViewBag.Specialties = await _professionalService.GetAllSpecialtiesAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SpecialtyId = specialtyId;
            return View(providers);
        }

        // GET: /Professional/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var provider = await _professionalService.GetProviderByIdAsync(id);
            if (provider == null) return NotFound();

            var reviews = await _professionalService.GetProviderReviewsAsync(id);
            ViewBag.Reviews = reviews;
            return View(provider);
        }

        // ============================
        // APPOINTMENTS
        // ============================

        // GET: /Professional/Book/{id}
        [HttpGet]
        public async Task<IActionResult> Book(Guid id)
        {
            var provider = await _professionalService.GetProviderByIdAsync(id);
            if (provider == null) return NotFound();

            ViewBag.ProviderId = id;
            ViewBag.Services = provider.Practices
                .SelectMany(p => p.Services)
                .ToList();
            ViewBag.Slots = provider.Practices
                .SelectMany(p => p.AvailabilitySlots)
                .ToList();

            return View(new CreateAppointmentRequest());
        }

        // POST: /Professional/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(CreateAppointmentRequest request)
        {
            // ============================================================
            // VALIDATION: SlotId must be provided
            // ============================================================
            if (request == null || request.SlotId == Guid.Empty)
            {
                TempData["Error"] = "Please select a time slot before booking.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields correctly.";
                return RedirectToAction(nameof(Index));
            }

            var userId = CurrentUserId;
            if (userId == Guid.Empty)
            {
                return Challenge();
            }

            try
            {
                await _professionalService.BookAppointmentAsync(userId, request);
                TempData["Success"] = "Appointment booked successfully!";
                return RedirectToAction(nameof(MyAppointments));
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "The selected slot is no longer available. Please try another.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Something went wrong while booking. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Professional/MyAppointments
        public async Task<IActionResult> MyAppointments()
        {
            var userId = CurrentUserId;
            if (userId == Guid.Empty)
                return Challenge();

            var appointments = await _professionalService.GetUserAppointmentsAsync(userId);
            return View(appointments);
        }

        // POST: /Professional/CancelAppointment/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "Invalid appointment.";
                return RedirectToAction(nameof(MyAppointments));
            }

            try
            {
                await _professionalService.CancelAppointmentAsync(id, CurrentUserId);
                TempData["Success"] = "Appointment cancelled.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Could not cancel the appointment. Please try again.";
            }

            return RedirectToAction(nameof(MyAppointments));
        }

        // ============================
        // REVIEWS
        // ============================

        // POST: /Professional/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(CreateReviewRequest request)
        {
            if (request == null || request.ProviderId == Guid.Empty)
            {
                TempData["Error"] = "Invalid review submission.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide a valid rating.";
                return RedirectToAction(nameof(Details), new { id = request.ProviderId });
            }

            await _professionalService.AddReviewAsync(CurrentUserId, request);
            TempData["Success"] = "Thank you for your review!";
            return RedirectToAction(nameof(Details), new { id = request.ProviderId });
        }

        // ============================
        // PROVIDER MANAGEMENT (Admin/Provider)
        // ============================

        // GET: /Professional/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Specialties = await _professionalService.GetAllSpecialtiesAsync();
            return View(new CreateProviderRequest());
        }

        // POST: /Professional/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProviderRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Specialties = await _professionalService.GetAllSpecialtiesAsync();
                return View(request);
            }

            request.UserId = CurrentUserId;
            var providerId = await _professionalService.CreateProviderAsync(request);
            TempData["Success"] = "Provider profile created successfully!";
            return RedirectToAction(nameof(Details), new { id = providerId });
        }

        // GET: /Professional/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var provider = await _professionalService.GetProviderByIdAsync(id);
            if (provider == null) return NotFound();

            var updateRequest = new UpdateProviderRequest
            {
                ProviderId = provider.ProviderId,
                Bio = provider.Bio,
                LicenseNumber = provider.LicenseNumber,
                YearsOfExperience = provider.YearsOfExperience,
                IsActive = provider.IsActive,
                SpecialtyIds = provider.Specialties
                    .Select(s => s.SpecialtyId)
                    .ToList()
            };

            ViewBag.Specialties = await _professionalService.GetAllSpecialtiesAsync();
            return View(updateRequest);
        }

        // POST: /Professional/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProviderRequest request)
        {
            if (request == null || request.ProviderId == Guid.Empty)
            {
                TempData["Error"] = "Invalid provider.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Specialties = await _professionalService.GetAllSpecialtiesAsync();
                return View(request);
            }

            await _professionalService.UpdateProviderAsync(request);
            TempData["Success"] = "Provider profile updated successfully!";
            return RedirectToAction(nameof(Details), new { id = request.ProviderId });
        }

        // ============================
        // MODERATION / VERIFICATION (Admin)
        // ============================

        // GET: /Professional/VerifyDocuments/{providerId}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyDocuments(Guid providerId)
        {
            var documents = await _professionalService.GetVerificationDocumentsAsync(providerId);
            ViewBag.ProviderId = providerId;
            return View(documents);
        }

        // POST: /Professional/UpdateDocumentStatus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDocumentStatus(Guid documentId, string status)
        {
            if (documentId == Guid.Empty || string.IsNullOrWhiteSpace(status))
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction(nameof(Index));
            }

            await _professionalService.UpdateVerificationDocumentStatusAsync(documentId, status);
            TempData["Success"] = "Document status updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}