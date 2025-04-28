using Tourest.Data.Entities;
using Tourest.ViewModels.Account;

namespace Tourest.Data.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> RegisterAccount(Account Account);
        Task<User> RegisterUser(User User);
        Task<User> CheckEmailexist(String email);
        Task<Account> GetAccountByID(int id);
        Task<bool> SetToken(string email, string token);
   
    
    }
}
