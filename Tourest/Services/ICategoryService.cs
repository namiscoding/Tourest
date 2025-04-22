using Tourest.ViewModels.Category;

namespace Tourest.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesForDisplayAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id);
    }
}
