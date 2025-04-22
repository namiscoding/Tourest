using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetGuideByIdAsync(int userId);
    }
}
