using Tourest.Data.Entities;
using Tourest.ViewModels.Category;

namespace Tourest.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<bool> CheckNameExistsAsync(string name, int? excludeCategoryId = null);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int categoryId);
        Task<bool> IsInUseAsync(int categoryId); // Kiểm tra Category có đang được Tour nào sử dụng không
        Task<Category?> GetByIdAsync(int id); Task<IEnumerable<CategoryViewModel>> GetAllForDisplayAsync(); // Giả sử đã có
        Task<IEnumerable<CategoryStatViewModel>> GetCategoryStatisticsAsync(); // Lấy thống kê
    }
}
