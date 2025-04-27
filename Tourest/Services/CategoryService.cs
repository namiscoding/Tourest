using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.ViewModels.Category;

namespace Tourest.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
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
            };
        }
        
        public async Task<(bool Success, string ErrorMessage)> CreateCategoryAsync(CategoryInputViewModel model)
        {
            _logger.LogInformation("Attempting to create category with name: {CategoryName}", model.Name);
            // Validate uniqueness
            if (await _categoryRepository.CheckNameExistsAsync(model.Name))
            {
                return (false, $"Tên danh mục '{model.Name}' đã tồn tại.");
            }

            // Map ViewModel to Entity (Ví dụ Manual Mapping)
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };
            // Hoặc dùng AutoMapper: var category = _mapper.Map<Category>(model);

            try
            {
                await _categoryRepository.AddAsync(category);
                _logger.LogInformation("Category '{CategoryName}' created successfully with ID {CategoryId}", category.Name, category.CategoryID);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category with name {CategoryName}", model.Name);
                return (false, "Đã có lỗi xảy ra khi tạo danh mục.");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Attempting to delete category with ID: {CategoryId}", categoryId);
            // Kiểm tra xem Category có đang được sử dụng không
            if (await _categoryRepository.IsInUseAsync(categoryId))
            {
                _logger.LogWarning("Cannot delete category {CategoryId} because it is in use by tours.", categoryId);
                return (false, "Không thể xóa danh mục này vì đang có Tour sử dụng.");
            }

            try
            {
                await _categoryRepository.DeleteAsync(categoryId);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", categoryId);
                return (false, "Đã có lỗi xảy ra khi xóa danh mục.");
            }
        }
        public async Task<CategoryInputViewModel?> GetCategoryForEditAsync(int categoryId)
        {
            _logger.LogInformation("Fetching category for edit by ID: {CategoryId}", categoryId);
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) return null;
            // Map Category to CategoryInputViewModel
            return new CategoryInputViewModel
            {
                CategoryID = category.CategoryID,
                Name = category.Name,
                Description = category.Description
            };
            // Hoặc dùng AutoMapper: return _mapper.Map<CategoryInputViewModel>(category);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateCategoryAsync(CategoryInputViewModel model)
        {
            _logger.LogInformation("Attempting to update category with ID: {CategoryId}", model.CategoryID);
            // Validate uniqueness (excluding self)
            if (await _categoryRepository.CheckNameExistsAsync(model.Name, model.CategoryID))
            {
                return (false, $"Tên danh mục '{model.Name}' đã tồn tại.");
            }

            var categoryToUpdate = await _categoryRepository.GetByIdAsync(model.CategoryID);
            if (categoryToUpdate == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for update.", model.CategoryID);
                return (false, "Không tìm thấy danh mục để cập nhật.");
            }

            // Map updated properties from ViewModel to Entity
            categoryToUpdate.Name = model.Name;
            categoryToUpdate.Description = model.Description;
            // Hoặc dùng AutoMapper: _mapper.Map(model, categoryToUpdate);

            try
            {
                await _categoryRepository.UpdateAsync(categoryToUpdate);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", model.CategoryID);
                return (false, "Đã có lỗi xảy ra khi cập nhật danh mục.");
            }
        }
            
        public async Task<List<CategoryStatViewModel>> GetCategoryStatisticsAsync()
        {
            var stats = await _categoryRepository.GetCategoryStatisticsAsync();
            // Xử lý trường hợp MinPrice là int.MaxValue (nghĩa là không có tour active nào)
            return stats.Select(s => {
                if (s.MinPrice == int.MaxValue) { s.MinPrice = 0; } // Gán lại là 0 nếu không có tour
                return s;
            }).ToList();
        }
    }
}
