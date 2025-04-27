using Tourest.ViewModels.Category;

namespace Tourest.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesForDisplayAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
        //Task<List<CategoryViewModel>> GetAllCategoriesForDisplayAsync(); // Giả sử đã có
        Task<List<CategoryStatViewModel>> GetCategoryStatisticsAsync();
    }
}
