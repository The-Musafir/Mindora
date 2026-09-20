using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Professional;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Notifications;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class ProfessionalService : IProfessionalService
    {
        private readonly MindoraDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        public ProfessionalService(MindoraDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        // ============================
        // PROVIDER DIRECTORY
        // ============================

        public async Task<IReadOnlyList<ProviderDto>> GetAllProvidersAsync(string? searchTerm = null, Guid? specialtyId = null)
        {
            var query = _context.ProfessionalProviders
                .Where(p => p.IsActive)
                .Include(p => p.User)
                .Include(p => p.SpecialtyMappings)
                    .ThenInclude(m => m.Specialty)
                .Include(p => p.PracticeDetails)
                    .ThenInclude(d => d.Services)
                .Include(p => p.PracticeDetails)
                    .ThenInclude(d => d.AvailabilitySlots)
                .Include(p => p.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.User.UserName.Contains(searchTerm) ||
                                         p.User.Email.Contains(searchTerm) ||
                                         p.Bio.Contains(searchTerm));
            }

            if (specialtyId.HasValue)
            {
                query = query.Where(p => p.SpecialtyMappings.Any(m => m.SpecialtyId == specialtyId.Value));
            }

            var providers = await query.ToListAsync();
            var result = new List<ProviderDto>();

            foreach (var provider in providers)
            {
                result.Add(await MapProviderToDtoAsync(provider));
            }

            return result;
        }

        public async Task<ProviderDto?> GetProviderByIdAsync(Guid providerId)
        {
            var provider = await _context.ProfessionalProviders
                .Where(p => p.ProviderId == providerId && p.IsActive)
                .Include(p => p.User)
                .Include(p => p.SpecialtyMappings)
                    .ThenInclude(m => m.Specialty)
                .Include(p => p.PracticeDetails)
                    .ThenInclude(d => d.Services)
                .Include(p => p.PracticeDetails)
                    .ThenInclude(d => d.AvailabilitySlots)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync();

            return provider == null ? null : await MapProviderToDtoAsync(provider);
        }

        // ============================
        // PROVIDER MANAGEMENT
        // ============================

        public async Task<Guid> CreateProviderAsync(CreateProviderRequest request)
        {
            var provider = new ProfessionalProvider
            {
                ProviderId = Guid.NewGuid(),
                UserId = request.UserId,
                Bio = request.Bio,
                LicenseNumber = request.LicenseNumber,
                YearsOfExperience = request.YearsOfExperience,
                IsVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var specialtyId in request.SpecialtyIds.Distinct())
            {
                provider.SpecialtyMappings.Add(new ProviderSpecialtyMapping
                {
                    ProviderId = provider.ProviderId,
                    SpecialtyId = specialtyId
                });
            }

            _context.ProfessionalProviders.Add(provider);
            await _context.SaveChangesAsync();
            return provider.ProviderId;
        }

        public async Task<ProviderDto> UpdateProviderAsync(UpdateProviderRequest request)
        {
            var provider = await _context.ProfessionalProviders
                .Include(p => p.SpecialtyMappings)
                .Include(p => p.User)
                .Include(p => p.PracticeDetails)
                    .ThenInclude(d => d.Services)
                .Include(p => p.PracticeDetails)
                    .ThenInclude(d => d.AvailabilitySlots)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.ProviderId == request.ProviderId);

            if (provider == null)
                throw new KeyNotFoundException($"Provider {request.ProviderId} not found.");

            provider.Bio = request.Bio;
            provider.LicenseNumber = request.LicenseNumber;
            provider.YearsOfExperience = request.YearsOfExperience;
            provider.IsActive = request.IsActive;
            provider.UpdatedAt = DateTime.UtcNow;

            provider.SpecialtyMappings.Clear();
            foreach (var specialtyId in request.SpecialtyIds.Distinct())
            {
                provider.SpecialtyMappings.Add(new ProviderSpecialtyMapping
                {
                    ProviderId = provider.ProviderId,
                    SpecialtyId = specialtyId
                });
            }

            await _context.SaveChangesAsync();
            return await MapProviderToDtoAsync(provider);
        }

        public async Task<bool> ToggleProviderActiveAsync(Guid providerId, bool isActive)
        {
            var provider = await _context.ProfessionalProviders.FindAsync(providerId);
            if (provider == null) return false;

            provider.IsActive = isActive;
            provider.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // SPECIALTY MANAGEMENT
        // ============================

        public async Task<IReadOnlyList<SpecialtyDto>> GetAllSpecialtiesAsync()
        {
            var specialties = await _context.ProviderSpecialties
                .Select(s => new SpecialtyDto
                {
                    SpecialtyId = s.SpecialtyId,
                    Name = s.Name,
                    Description = s.Description
                })
                .ToListAsync();

            return specialties;
        }

        public async Task<Guid> CreateSpecialtyAsync(SpecialtyDto dto)
        {
            var specialty = new ProviderSpecialty
            {
                SpecialtyId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            _context.ProviderSpecialties.Add(specialty);
            await _context.SaveChangesAsync();
            return specialty.SpecialtyId;
        }

        // ============================
        // PRACTICE / SERVICE / AVAILABILITY
        // ============================

        public async Task<Guid> CreatePracticeAsync(CreatePracticeRequest request)
        {
            var practice = new ProviderPracticeDetail
            {
                PracticeId = Guid.NewGuid(),
                ProviderId = request.ProviderId,
                PracticeName = request.PracticeName,
                Address = request.Address,
                Phone = request.Phone,
                IsVirtual = request.IsVirtual
            };

            _context.ProviderPracticeDetails.Add(practice);
            await _context.SaveChangesAsync();
            return practice.PracticeId;
        }

        public async Task<Guid> AddServiceAsync(CreateServiceRequest request)
        {
            var service = new ProviderService
            {
                ServiceId = Guid.NewGuid(),
                PracticeId = request.PracticeId,
                ServiceName = request.ServiceName,
                DurationMinutes = request.DurationMinutes,
                Fee = request.Fee
            };

            _context.ProviderServices.Add(service);
            await _context.SaveChangesAsync();
            return service.ServiceId;
        }

        public async Task<Guid> AddAvailabilitySlotAsync(CreateAvailabilitySlotRequest request)
        {
            var slot = new ProviderAvailabilitySlot
            {
                SlotId = Guid.NewGuid(),
                PracticeId = request.PracticeId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsRecurring = request.IsRecurring,
                SpecificDate = request.SpecificDate
            };

            _context.ProviderAvailabilitySlots.Add(slot);
            await _context.SaveChangesAsync();
            return slot.SlotId;
        }

        // ============================
        // APPOINTMENTS
        // ============================

        public async Task<AppointmentDto> BookAppointmentAsync(Guid userId, CreateAppointmentRequest request)
        {
            var slot = await _context.ProviderAvailabilitySlots.FindAsync(request.SlotId);
            if (slot == null)
                throw new KeyNotFoundException("Slot not found.");

            var appointment = new Appointment
            {
                AppointmentId = Guid.NewGuid(),
                SlotId = request.SlotId,
                UserId = userId,
                ServiceId = request.ServiceId,
                Status = "Scheduled",
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);

            appointment.StatusHistories.Add(new AppointmentStatusHistory
            {
                HistoryId = Guid.NewGuid(),
                AppointmentId = appointment.AppointmentId,
                Status = "Scheduled",
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var fullAppointment = await _context.Appointments
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Practice)
                        .ThenInclude(p => p.Provider)
                            .ThenInclude(pr => pr.User)
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.StatusHistories)
                .FirstAsync(a => a.AppointmentId == appointment.AppointmentId);

            // ============================================================
            // NOTIFICATION: Appointment confirmed
            // ============================================================
            try
            {
                var providerName = fullAppointment.Slot?.Practice?.Provider?.User?.UserName
                                   ?? fullAppointment.Slot?.Practice?.Provider?.User?.Email
                                   ?? "your provider";

                // AppointmentDate is DateOnly — convert to DateTime for notification DTO
                var appointmentDateTime = fullAppointment.AppointmentDate.ToDateTime(TimeOnly.MinValue);

                var notif = MindoraNotifications.AppointmentConfirmed(
                    providerName,
                    appointmentDateTime,
                    fullAppointment.AppointmentId);

                await _dispatcher.DispatchAsync(userId, notif);
            }
            catch
            {
                // Silent fail — appointment already booked
            }

            return MapAppointmentToDto(fullAppointment);
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetUserAppointmentsAsync(Guid userId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Practice)
                        .ThenInclude(p => p.Provider)
                            .ThenInclude(pr => pr.User)
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.StatusHistories)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return appointments.Select(MapAppointmentToDto).ToList();
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetProviderAppointmentsAsync(Guid providerId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.Slot.Practice.ProviderId == providerId)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Practice)
                        .ThenInclude(p => p.Provider)
                            .ThenInclude(pr => pr.User)
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.StatusHistories)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return appointments.Select(MapAppointmentToDto).ToList();
        }

        public async Task<bool> UpdateAppointmentStatusAsync(Guid appointmentId, string newStatus)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Status = newStatus;
            _context.AppointmentStatusHistories.Add(new AppointmentStatusHistory
            {
                HistoryId = Guid.NewGuid(),
                AppointmentId = appointmentId,
                Status = newStatus,
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAppointmentAsync(Guid appointmentId, Guid userId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null || appointment.UserId != userId) return false;

            appointment.Status = "Cancelled";
            _context.AppointmentStatusHistories.Add(new AppointmentStatusHistory
            {
                HistoryId = Guid.NewGuid(),
                AppointmentId = appointmentId,
                Status = "Cancelled",
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // REVIEWS
        // ============================

        public async Task<Guid> AddReviewAsync(Guid userId, CreateReviewRequest request)
        {
            var review = new Review
            {
                ReviewId = Guid.NewGuid(),
                ProviderId = request.ProviderId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review.ReviewId;
        }

        public async Task<IReadOnlyList<ReviewDto>> GetProviderReviewsAsync(Guid providerId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProviderId == providerId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewDto
            {
                ReviewId = r.ReviewId,
                ProviderId = r.ProviderId,
                UserId = r.UserId,
                ReviewerName = r.User.UserName ?? r.User.Email,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<double?> GetProviderAverageRatingAsync(Guid providerId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.ProviderId == providerId)
                .Select(r => (double?)r.Rating)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : null;
        }

        // ============================
        // VERIFICATION
        // ============================

        public async Task<Guid> UploadVerificationDocumentAsync(Guid providerId, ProviderVerificationDocumentDto dto)
        {
            var document = new ProviderVerificationDocument
            {
                DocumentId = Guid.NewGuid(),
                ProviderId = providerId,
                DocumentType = dto.DocumentType,
                FileUrl = dto.FileUrl,
                Status = "Pending"
            };

            _context.ProviderVerificationDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document.DocumentId;
        }

        public async Task<IReadOnlyList<ProviderVerificationDocumentDto>> GetVerificationDocumentsAsync(Guid providerId)
        {
            var documents = await _context.ProviderVerificationDocuments
                .Where(d => d.ProviderId == providerId)
                .ToListAsync();

            return documents.Select(d => new ProviderVerificationDocumentDto
            {
                DocumentId = d.DocumentId,
                ProviderId = d.ProviderId,
                DocumentType = d.DocumentType,
                FileUrl = d.FileUrl,
                VerifiedAt = d.VerifiedAt,
                Status = d.Status
            }).ToList();
        }

        public async Task<bool> UpdateVerificationDocumentStatusAsync(Guid documentId, string status)
        {
            var document = await _context.ProviderVerificationDocuments.FindAsync(documentId);
            if (document == null) return false;

            document.Status = status;
            document.VerifiedAt = status == "Approved" ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // PRIVATE MAPPING
        // ============================

        private async Task<ProviderDto> MapProviderToDtoAsync(ProfessionalProvider provider)
        {
            double? avgRating = provider.Reviews != null && provider.Reviews.Any()
                ? provider.Reviews.Average(r => r.Rating)
                : null;

            return new ProviderDto
            {
                ProviderId = provider.ProviderId,
                UserId = provider.UserId,
                FullName = provider.User?.UserName ?? provider.User?.Email ?? "Provider",
                Email = provider.User?.Email ?? "",
                Bio = provider.Bio,
                LicenseNumber = provider.LicenseNumber,
                YearsOfExperience = provider.YearsOfExperience,
                IsVerified = provider.IsVerified,
                IsActive = provider.IsActive,
                AverageRating = avgRating,
                ReviewCount = provider.Reviews?.Count ?? 0,
                Specialties = provider.SpecialtyMappings.Select(m => new SpecialtyDto
                {
                    SpecialtyId = m.Specialty.SpecialtyId,
                    Name = m.Specialty.Name,
                    Description = m.Specialty.Description
                }).ToList(),
                Practices = provider.PracticeDetails.Select(d => new PracticeDetailDto
                {
                    PracticeId = d.PracticeId,
                    PracticeName = d.PracticeName,
                    Address = d.Address,
                    Phone = d.Phone,
                    IsVirtual = d.IsVirtual,
                    Services = d.Services.Select(s => new ServiceDto
                    {
                        ServiceId = s.ServiceId,
                        ServiceName = s.ServiceName,
                        DurationMinutes = s.DurationMinutes,
                        Fee = s.Fee
                    }).ToList(),
                    AvailabilitySlots = d.AvailabilitySlots.Select(s => new AvailabilitySlotDto
                    {
                        SlotId = s.SlotId,
                        DayOfWeek = s.DayOfWeek,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsRecurring = s.IsRecurring,
                        SpecificDate = s.SpecificDate
                    }).ToList()
                }).ToList()
            };
        }

        private AppointmentDto MapAppointmentToDto(Appointment appointment)
        {
            return new AppointmentDto
            {
                AppointmentId = appointment.AppointmentId,
                SlotId = appointment.SlotId,
                UserId = appointment.UserId,
                PatientName = appointment.User?.UserName ?? appointment.User?.Email ?? "Patient",
                ServiceId = appointment.ServiceId,
                ServiceName = appointment.Service?.ServiceName,
                Status = appointment.Status,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt,
                StatusHistory = appointment.StatusHistories.Select(h => new AppointmentStatusHistoryDto
                {
                    HistoryId = h.HistoryId,
                    Status = h.Status,
                    ChangedAt = h.ChangedAt
                }).ToList()
            };
        }
    }
}