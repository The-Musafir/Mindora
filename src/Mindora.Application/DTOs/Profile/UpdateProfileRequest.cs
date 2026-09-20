using System.ComponentModel.DataAnnotations;

namespace Mindora.Application.DTOs.Profile
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "Display name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        [Display(Name = "Full Name")]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Timezone")]
        public string? Timezone { get; set; }
    }
}