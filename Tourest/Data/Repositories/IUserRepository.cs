using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface IUserRepository
    {
        Task<(IEnumerable<User> Users, int TotalCount)> GetUsersByRolePagedAsync(string roleName, int pageIndex = 1, int pageSize = 10, string searchTerm = "");
        Task<User?> GetUserWithAccountByIdAsync(int userId);
        Task<User?> FindByIdAsync(int userId);
        Task<bool> CheckEmailExistsAsync(string email, int? excludeUserId = null);
        Task<bool> CheckUsernameExistsAsync(string username, int? excludeUserId = null);
        Task<Account?> FindAccountByUsernameAsync(string username);
        Task<(bool Success, User? CreatedUser)> AddUserAndAccountAsync(User user, Account account); // Trả về User đã tạo
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateAccountAsync(Account account);
        Task<User?> GetGuideByIdAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> UpdateFullNameAndAddressAsync(int userId, string fullName, string address);
    }
}
