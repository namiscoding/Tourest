using Tourest.Data.Entities;
using Tourest.ViewModels.Category;

namespace Tourest.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id); Task<IEnumerable<CategoryViewModel>> GetAllForDisplayAsync(); // Giả sử đã có
        Task<IEnumerable<CategoryStatViewModel>> GetCategoryStatisticsAsync(); // Lấy thống kê
    }
}
