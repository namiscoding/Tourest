using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Tourest.ViewModels.Account
{
    public class AuthenticationViewModel
    {
        public LoginModel Login { get; set; } = new LoginModel();
        [ValidateNever]
        public RegisterModel Register { get; set; } = new RegisterModel();
        
    }

}
