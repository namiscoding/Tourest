using Tourest.Data.Repositories;
using Tourest.ViewModels.Category;

namespace Tourest.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        // Inject thêm các service khác nếu cần

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategoriesForDisplayAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            // Map từ Category entity sang CategoryViewModel
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                CategoryId = c.CategoryID,
                Name = c.Name
                // Map các thuộc tính khác nếu cần
            }).ToList();

            return categoryViewModels;
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryViewModel
            {
                CategoryId = category.CategoryID,
                Name = category.Name
                // Map các thuộc tính khác
            };
        }
    }
}
