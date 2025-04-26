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

        public async Task<IEnumerable<Tour>> GetActiveToursAsync(
            IEnumerable<int>? categoryIds = null,
            IEnumerable<string>? destinations = null,
            IEnumerable<int>? ratings = null,
            string? sortBy = null, 
            int? minPrice = null, // THÊM
            int? maxPrice = null,
             // --- THÊM THAM SỐ ---
             string? searchDestination = null,
             string? searchCategoryName = null,
             DateTime? searchDate = null, // Nhận nhưng chưa lọc
             int? searchGuests = null)
        {
            var query = _context.Tours
                                .Where(t => t.Status == "Active")
                                // KHÔNG dùng AsNoTracking() ở đây vội nếu cần navigation properties để tính toán sau đó
                                ;
            query = query
         .Include(t => t.TourRatings).ThenInclude(tr => tr.Rating)
         .Include(t => t.TourCategories).ThenInclude(tc => tc.Category);
            // --- Áp dụng Filters ---
            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(t => t.TourCategories.Any(tc => categoryIds.Contains(tc.CategoryID)));
            }
            if (destinations != null && destinations.Any())
            {
                query = query.Where(t => destinations.Contains(t.Destination));
            }
            if (ratings != null && ratings.Any())
            {
                var minRating = (decimal)ratings.Max(); // Ép kiểu sang decimal để so sánh
                                                        // Lọc những tour CÓ rating VÀ rating trung bình TÍNH TOÁN được >= minRating
                query = query.Where(t => t.TourRatings.Any() && // Phải có ít nhất 1 rating
                                         t.TourRatings.Average(tr => tr.Rating.RatingValue) >= minRating);
            }
            if (minPrice.HasValue)
            {
                // Lọc theo giá người lớn (hoặc giá trẻ em tùy yêu cầu)
                query = query.Where(t => t.ChildPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(t => t.ChildPrice <= maxPrice.Value);
            }
            if (!string.IsNullOrWhiteSpace(searchDestination))
            {
                // Tìm kiếm gần đúng trong Destination (không phân biệt hoa thường tùy DB collation)
                query = query.Where(t => t.Destination.Contains(searchDestination));
            }
            if (!string.IsNullOrWhiteSpace(searchCategoryName))
            {
                // Tìm kiếm gần đúng trong tên Category (cần Include ở trên)
                query = query.Where(t => t.TourCategories.Any(tc => tc.Category.Name.Contains(searchCategoryName)));
            }
            if (searchGuests.HasValue && searchGuests.Value > 0)
            {
                // Lọc tour có MaxGroupSize null (không giới hạn) hoặc >= số khách yêu cầu
                query = query.Where(t => t.MaxGroupSize == null || t.MaxGroupSize >= searchGuests.Value);
            }
            // !! Lọc theo searchDate cần thay đổi Schema DB để lưu ngày khởi hành/khoảng thời gian hoạt động của Tour !!
            // if (searchDate.HasValue) { /* Logic lọc theo ngày (ví dụ: nếu có bảng TourDepartures) */ }
            // --- KẾT THÚC FILTER MỚI ---
            // --- Tải kèm dữ liệu liên quan ---
            // Include TourRatings và Rating sau khi lọc để tính toán ở Service
            query = query
                    .Include(t => t.TourRatings) // Tải kèm danh sách TourRating
                        .ThenInclude(tr => tr.Rating); // Từ mỗi TourRating, tải kèm Rating gốc chứa RatingValue

            // --- Sắp xếp ---
            var filteredTours = await query.AsNoTracking().ToListAsync();
            IEnumerable<Tour> sortedTours;

            switch (sortBy?.ToLowerInvariant())
            {
                case "price_asc":
                    sortedTours = filteredTours.OrderBy(t => t.ChildPrice);
                    break;
                case "price_desc":
                    sortedTours = filteredTours.OrderByDescending(t => t.ChildPrice);
                    break;
                case "rating_desc": // Sắp xếp theo rating tính toán
                    sortedTours = filteredTours.OrderByDescending(t =>
                        (t.TourRatings != null && t.TourRatings.Any()) ? t.TourRatings.Average(tr => tr.Rating.RatingValue) : 0m);
                    break;
                case "duration_asc": // Giữ lại nếu cần
                    sortedTours = filteredTours.OrderBy(t => t.DurationDays);
                    break;
                case "name_asc": // Thêm case cho rõ ràng
                    sortedTours = filteredTours.OrderBy(t => t.Name);
                    break;
                case "name_desc": // THÊM CASE NÀY
                    sortedTours = filteredTours.OrderByDescending(t => t.Name);
                    break;
                default: // Mặc định là Name A-Z
                    sortedTours = filteredTours.OrderBy(t => t.Name);
                    break;
            }
            // Áp dụng AsNoTracking() cuối cùng trước khi ToListAsync
            return sortedTours;
        }

        public async Task<IEnumerable<string>> GetDistinctActiveDestinationsAsync()
        {
            return await _context.Tours
                                 .Where(t => t.Status == "Active" && !string.IsNullOrEmpty(t.Destination)) // Chỉ lấy tour active và có destination
                                 .Select(t => t.Destination) // Chọn cột Destination
                                 .Distinct()                 // Lấy các giá trị duy nhất
                                 .OrderBy(d => d)            // Sắp xếp theo alphabet
                                 .ToListAsync();
        }

        public async Task<Tour?> GetByIdAsync(int id)
        {
            return await _context.Tours
                .Where(t => t.TourID == id && t.Status == "Active") // Thêm kiểm tra Status nếu chỉ muốn xem tour active
                .Include(t => t.ItineraryDays.OrderBy(it => it.DayNumber)) // Tải lịch trình và sắp xếp theo ngày
                .Include(t => t.TourCategories) // Tải bảng nối TourCategory
                    .ThenInclude(tc => tc.Category) // Từ TourCategory tải Category liên quan
                .Include(t => t.TourRatings) // Tải bảng nối TourRating
                    .ThenInclude(tr => tr.Rating) // Từ TourRating tải Rating gốc
                        .ThenInclude(r => r.Customer) // Từ Rating gốc tải User (Customer)
                .AsNoTracking() // Vì chỉ đọc dữ liệu
                .FirstOrDefaultAsync(); // Lấy tour đầu tiên hoặc null
        }
    }
}
