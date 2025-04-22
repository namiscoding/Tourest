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
    }
}
