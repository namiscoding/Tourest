using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Account
{
    public class RegisterModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
    
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = string.Empty;
     
        [DataType(DataType.Password)]
        public string PasswordHashConfirm { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
        public override string ToString()
        {
            return $"Email: {Email}, PhoneNumber: {PhoneNumber}, ReturnUrl: {ReturnUrl}";
        }
    }
}
