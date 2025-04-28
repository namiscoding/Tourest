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
        Task<bool> UpdatePassword(int uid, string newP, string currenPass);
        Task<bool> AddtokenForgot(string token, string email);
    }
}
