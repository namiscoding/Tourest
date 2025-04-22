using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger; 

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersByRolePagedAsync(string roleName, int pageIndex = 1, int pageSize = 10, string searchTerm = "")
        {
            var query = _context.Users
                              .Include(u => u.Account) // Include Account để lọc Role
                              .Where(u => u.Account != null && u.Account.Role == roleName);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(u => (u.FullName != null && u.FullName.ToLower().Contains(searchTerm)) ||
                                         (u.Email != null && u.Email.ToLower().Contains(searchTerm)) ||
                                         (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))); // Tìm gần đúng
            }

            var totalCount = await query.CountAsync();
            var users = await query.OrderBy(u => u.FullName) // Sắp xếp
                                   .Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (users, totalCount);
        }


        public async Task<User?> GetUserWithAccountByIdAsync(int userId)
        {
            return await _context.Users
                                 .Include(u => u.Account)
                                 .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<User?> FindByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> CheckEmailExistsAsync(string email, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var query = _context.Users.Where(u => u.Email.ToLower() == email.ToLower());
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserID != excludeUserId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> CheckUsernameExistsAsync(string username, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            var query = _context.Accounts.Where(a => a.Username.ToLower() == username.ToLower());
            if (excludeUserId.HasValue)
            {
                query = query.Where(a => a.UserID != excludeUserId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<Account?> FindAccountByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            return await _context.Accounts
                                 .Include(a => a.User) // Lấy kèm User nếu cần check IsActive
                                 .FirstOrDefaultAsync(a => a.Username.ToLower() == username.ToLower());
        }

        public async Task<(bool Success, User? CreatedUser)> AddUserAndAccountAsync(User user, Account account)
        {
            // Dùng transaction để đảm bảo cả 2 được thêm hoặc không
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Thêm User trước để lấy UserID
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync(); // Lưu để UserID được tạo ra

                // Gán UserID cho Account và thêm Account
                account.UserID = user.UserID;
                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully added User {UserId} and Account {AccountId}", user.UserID, account.AccountID);
                return (true, user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error adding User and Account for User {Username}", account.Username);
                return (false, null);
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex) // Xử lý lỗi concurrency nếu cần
            {
                _logger.LogError(ex, "Concurrency error updating User {UserId}", user.UserID);
                // Tải lại entity hoặc xử lý theo logic nghiệp vụ
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating User {UserId}", user.UserID);
                return false;
            }
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            try
            {
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating Account {AccountId}", account.AccountID);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Account {AccountId}", account.AccountID);
                return false;
            }
        }

        // Bỏ qua Delete cứng, ưu tiên Update IsActive
    }
}
