using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
using Tourest.ViewModels.Category;

namespace Tourest.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(ApplicationDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task AddAsync(Category category)
        {
            try
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Added new Category with ID {CategoryId}", category.CategoryID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Category with Name {CategoryName}", category.Name);
                throw; 
            }
        }

        public async Task<bool> CheckNameExistsAsync(string name, int? excludeCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());
            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryID != excludeCategoryId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task DeleteAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                try
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Deleted Category with ID {CategoryId}", categoryId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting Category with ID {CategoryId}", categoryId);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent Category with ID {CategoryId}", categoryId);
            }
        }
        public async Task<bool> IsInUseAsync(int categoryId)
        {
            return await _context.TourCategories.AnyAsync(tc => tc.CategoryID == categoryId);
        }

        public async Task UpdateAsync(Category category)
        {
            var local = _context.Set<Category>()
                .Local
                .FirstOrDefault(entry => entry.CategoryID.Equals(category.CategoryID));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated Category with ID {CategoryId}", category.CategoryID);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating Category {CategoryId}", category.CategoryID);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Category {CategoryId}", category.CategoryID);
                throw;
            }
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                                 .OrderBy(c => c.Name)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.CategoryID == id);
        }
        public async Task<IEnumerable<CategoryViewModel>> GetAllForDisplayAsync()
        {
            return await _context.Categories
                           .OrderBy(c => c.Name)
                           .Select(c => new CategoryViewModel { CategoryId = c.CategoryID, Name = c.Name })
                           .AsNoTracking()
                           .ToListAsync();
        }

        // File: Repositories/CategoryRepository.cs
        public async Task<IEnumerable<CategoryStatViewModel>> GetCategoryStatisticsAsync()
        {
            // Bước 1: Lấy dữ liệu cần thiết từ DB
            // Chọn ra CategoryName và danh sách giá của các tour active thuộc category đó
            var categoriesData = await _context.Categories
                .Where(c => c.TourCategories.Any(tc => tc.Tour.Status == "Active")) // Vẫn lọc category có tour active ở DB
                .Select(c => new // Tạo kiểu dữ liệu tạm thời để chứa kết quả
                {
                    CategoryName = c.Name,
                    // Lấy danh sách giá AdultPrice của các tour active trong category này
                    ActiveTourPrices = c.TourCategories
                                         .Where(tc => tc.Tour.Status == "Active")
                                         .Select(tc => tc.Tour.AdultPrice)
                                         // Quan trọng: ToList() để ép EF Core thực thi phần này và trả về List<int>
                                         .ToList()
                })
                 .AsNoTracking()
                 .ToListAsync(); // Thực thi truy vấn chính để lấy dữ liệu về client

            // Bước 2: Tính toán Count và Min trong bộ nhớ ứng dụng
            var results = categoriesData.Select(c => new CategoryStatViewModel
            {
                CategoryName = c.CategoryName,
                // Đếm số lượng phần tử trong danh sách giá đã lấy về
                TourCount = c.ActiveTourPrices.Count,
                // Tìm giá trị nhỏ nhất trong danh sách giá, trả về 0 nếu danh sách rỗng
                MinPrice = c.ActiveTourPrices.Any() ? c.ActiveTourPrices.Min() : 0
            })
             .OrderBy(s => s.CategoryName) // Sắp xếp kết quả cuối cùng
             .ToList(); // Chuyển thành List để trả về (hoặc giữ IEnumerable)

            return results;
        }
    }
}
