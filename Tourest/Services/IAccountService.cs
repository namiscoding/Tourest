using Tourest.Data.Entities;
using Tourest.ViewModels.Account;

namespace Tourest.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<UserViewModel>> AuthenticationAsync();
        Task<UserViewModel> RegisterAsync(RegisterModel AccountRegister);
        Task<User?> CheckEmailAsync(string email);
        Task<UserViewModel> UpdateProfile(UserViewModel userViewModel);

        
    }
}
