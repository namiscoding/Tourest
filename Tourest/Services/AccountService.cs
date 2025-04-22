using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Util;
using Tourest.ViewModels.Account;

namespace Tourest.Services
{
    public class AccountService : IAccountService
    {
        public readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task<IEnumerable<UserViewModel>> AuthenticationAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserViewModel> RegisterAsync(RegisterModel accountRegister)
        { 
          accountRegister.PasswordHash= BCrypt.Net.BCrypt.HashPassword(accountRegister.PasswordHash);
            // Chuyển đổi RegisterModel thành User
            User user = AccountMapper.RegisterModelToUser(accountRegister);
            Console.WriteLine(user.ToString());
            // Thêm User vào cơ sở dữ liệu
            User userInsert = await _accountRepository.RegisterUser(user);
            Console.WriteLine("AFTER INSERT: " + userInsert.UserID); // Đây mới là giá trị đúng

            // Chuyển đổi RegisterModel thành Account
            Account newAccount = AccountMapper.RegisterModeltoAccount(accountRegister,userInsert);
     
            Console.WriteLine(newAccount.ToString());
            // Thêm Account vào cơ sở dữ liệu
            Account accountInsert = await _accountRepository.RegisterAccount(newAccount);

            Console.WriteLine(accountInsert.ToString());
            // Chuyển đổi kết quả thành UserViewModel để trả về
            UserViewModel userViewModel = AccountMapper.ToUserViewModel(userInsert, accountInsert);

            return userViewModel;
        }

        public async Task<User?> CheckEmailAsync(string email)
        {
            User check = await _accountRepository.CheckEmailexist(email);

            return check != null ? check : null;
        }

   
    }
}
