using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Util;
using Tourest.ViewModels.Account;

namespace Tourest.Services
{
    public class AccountService : IAccountService
    {
        public readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;

        

        public AccountService(IAccountRepository accountRepository, IUserRepository userRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
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

        public Task<UserViewModel> UpdateProfile(UserViewModel userViewModel)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePassword(int uid ,string newP,string currenPass)
        {
            Console.Write(uid + " " + newP);
            bool status = await _userRepository.ChangePassword(uid, newP, currenPass);
            // Băm mật khẩu mới
            return status;
        }

        public async Task<bool> AddtokenForgot(string token, string email)
        {
            bool status = await _accountRepository.SetToken(email, token);
            return status;
        }
    }
}
