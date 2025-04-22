using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
    }
}
