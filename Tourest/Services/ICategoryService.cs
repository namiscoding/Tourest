using Tourest.ViewModels.Category;

namespace Tourest.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesForDisplayAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
        Task<(bool Success, string ErrorMessage)> CreateCategoryAsync(CategoryInputViewModel model);
        Task<CategoryInputViewModel?> GetCategoryForEditAsync(int categoryId);
        Task<(bool Success, string ErrorMessage)> UpdateCategoryAsync(CategoryInputViewModel model);
        Task<(bool Success, string ErrorMessage)> DeleteCategoryAsync(int categoryId);
        //Task<List<CategoryViewModel>> GetAllCategoriesForDisplayAsync(); // Giả sử đã có
        Task<List<CategoryStatViewModel>> GetCategoryStatisticsAsync();
    }
}
