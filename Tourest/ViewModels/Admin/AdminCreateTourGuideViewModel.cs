using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin
{
    public class AdminCreateTourGuideViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Sẽ dùng làm Username

        [DataType(DataType.Password)]
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; } = true;

        // Tour Guide Profile Fields
        [Display(Name = "Experience Level")]
        [StringLength(50)]
        public string? ExperienceLevel { get; set; }

        [Display(Name = "Languages Spoken (separate with ';')")]
        [StringLength(500)]
        public string? LanguagesSpoken { get; set; }

        [Display(Name = "Specializations (separate with ';')")]
        [StringLength(500)]
        public string? Specializations { get; set; }

        [Display(Name = "Max Group Size Capacity")]
        [Range(1, 1000)] // Example range
        public int? MaxGroupSizeCapacity { get; set; }
    }
}
