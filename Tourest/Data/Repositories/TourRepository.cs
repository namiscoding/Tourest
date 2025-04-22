using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;

namespace Tourest.Data.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly ApplicationDbContext _context;

        public TourRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tour>> GetActiveToursAsync()
        {
            // Logic truy vấn CSDL để lấy các tour có Status = "Active"
            return await _context.Tours
                                 .Where(t => t.Status == "Active") // Lọc theo trạng thái
                                 .OrderBy(t => t.Name)          // Sắp xếp theo tên (ví dụ)
                                 .AsNoTracking()                 // Tùy chọn: Tăng hiệu năng nếu chỉ đọc
                                 .ToListAsync();
        }
        public Task<Tour?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
