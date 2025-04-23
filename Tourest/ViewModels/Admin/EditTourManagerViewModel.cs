using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin
{
    public class EditTourManagerViewModel
    {
        [Required]
        public int UserId { get; set; } // Cần Id để biết sửa ai

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Cẩn thận khi cho sửa Email/Username

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
    }
}
