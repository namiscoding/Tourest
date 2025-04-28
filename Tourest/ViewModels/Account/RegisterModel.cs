using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tourest.ViewModels.Account
{
    public class RegisterModel : IValidatableObject 
    {
        [Required(ErrorMessage = "Email không được bỏ trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
     
        [Display(Name = "Email")]

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Họ tên không được bỏ trống.")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự.")]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("PasswordHash", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string PasswordHashConfirm { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        public override string ToString()
        {
            return $"Email: {Email}, PhoneNumber: {PhoneNumber}, ReturnUrl: {ReturnUrl}";
        }

        // Custom validate thêm: kiểm tra PhoneNumber và Email phải kết thúc bằng .com
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            // 1. Check PhoneNumber chỉ toàn số
            if (!string.IsNullOrEmpty(PhoneNumber) && !Regex.IsMatch(PhoneNumber, @"^\d+$"))
            {
                errors.Add(new ValidationResult("Số điện thoại chỉ được chứa các chữ số (0-9).", new[] { nameof(PhoneNumber) }));
            }

            // 2. Check Email phải kết thúc bằng .com
            if (!string.IsNullOrEmpty(Email) && !Email.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new ValidationResult("Email phải kết thúc bằng '.com'.", new[] { nameof(Email) }));
            }

            return errors;
        }
    }
}

