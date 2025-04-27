using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
using Tourest.ViewModels.Category;

namespace Tourest.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
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
