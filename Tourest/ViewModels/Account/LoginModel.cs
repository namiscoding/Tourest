using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Account
{
    public class LoginModel
    {
      
     
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
       
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = string.Empty;
        
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }

    }
}
