using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin
{
    public class EditTourGuideViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

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
        [Range(1, 1000)]
        public int? MaxGroupSizeCapacity { get; set; }
        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Ảnh đại diện mới (Chọn file nếu muốn thay đổi)")]
        public IFormFile? ProfilePictureFile { get; set; } // Nhận file upload mới
    }
}
