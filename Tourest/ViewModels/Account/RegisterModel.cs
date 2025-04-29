using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tourest.ViewModels.Account
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email cannot be empty.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|vn)$", ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number cannot be empty.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits (0-9).")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name cannot be empty.")]
        [Display(Name = "Full Name")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("PasswordHash", ErrorMessage = "Password confirmation does not match.")]
        public string PasswordHashConfirm { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        public override string ToString()
        {
            return $"Email: {Email}, PhoneNumber: {PhoneNumber}, ReturnUrl: {ReturnUrl}";
        }
    }
}
