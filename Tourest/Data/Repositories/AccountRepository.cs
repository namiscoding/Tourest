
using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
using Tourest.ViewModels.Account;

namespace Tourest.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> CheckEmailexist(string email)
        {
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Username == email);

            Console.WriteLine(account?.User?.Email);

            if (account != null && account.User != null)
                return account.User;

            return null;
        }

        public async Task<Account> GetAccountByID(int id)
        {
            var account = await _context.Accounts
                                        .Include(a => a.User) // Nếu muốn lấy kèm User
                                        .FirstOrDefaultAsync(a => a.AccountID == id);

            if (account == null)
            {
                throw new Exception("Không tìm thấy tài khoản!");
            }

            return account;
        }
        public Task<User> getUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Account> RegisterAccount(Account account)
        {
            _context.Accounts.Add(account); // Thêm bản ghi mới
            await _context.SaveChangesAsync(); // Lưu vào DB
            return account;
        }

        public async Task<User> RegisterUser(User User)
        {
            _context.Users.Add(User); // Thêm bản ghi mới
                await _context.SaveChangesAsync(); // Lưu vào DB
            return User;
        }

        public async Task<bool> SetToken(string email, string token)
        {
            var acccount = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == email);
            if (acccount == null)
            {
                return false;
            }

            acccount.PasswordResetToken = token;
            acccount.ResetTokenExpiration = DateTime.UtcNow.AddDays(7); // Token có hiệu lực trong 30 phút
     
            await _context.SaveChangesAsync();
            return true;


        }

      
    }
}
