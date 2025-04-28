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
        public async Task<IEnumerable<Tour>> GetFeaturedToursAsync(int count)
        {
            return await _context.Tours
                .Where(t => t.Status == "Active") // Chỉ lấy tour active
                                                  // Sắp xếp theo điểm trung bình giảm dần (cao nhất trước)
                                                  // Coi điểm null là thấp nhất (-1 hoặc một giá trị rất nhỏ)
                .OrderByDescending(t => t.AverageRating ?? -1m)
                .Take(count) // Lấy số lượng tour theo yêu cầu
                             // Include dữ liệu cần thiết cho việc mapping sang TourListViewModel trong Service
                             // Quan trọng: Include TourRatings để Service tính toán rating đồng nhất
                .Include(t => t.TourRatings)
                    .ThenInclude(tr => tr.Rating)
                .AsNoTracking()
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

        public async Task<int> GetActiveTourCountAsync()
        {
            return await _context.Tours.CountAsync(t => t.Status == "Active");
        }

        public async Task<int> GetDistinctDestinationCountAsync()
        {
            return await _context.Tours
                             .Where(t => t.Status == "Active" && !string.IsNullOrEmpty(t.Destination))
                             .Select(t => t.Destination)
                             .Distinct()
                             .CountAsync();
        }

        public async Task<Tour> AddTourAsync(Tour tour)
        {
            await _context.Tours.AddAsync(tour);
            await _context.SaveChangesAsync(); // Lưu để lấy TourID
            return tour;
        }

        public async Task DeleteTourAsync(int tourId)
        {
            var tour = await _context.Tours
                                     .Include(t => t.ItineraryDays) // Include để xóa kèm
                                     .Include(t => t.TourCategories) // Include để xóa kèm
                                     .FirstOrDefaultAsync(t => t.TourID == tourId);
            if (tour != null)
            {
                // EF Core thường tự xóa các entity phụ thuộc nếu được cấu hình Cascade
                // Hoặc bạn có thể xóa thủ công ở đây nếu không dùng cascade
                // _context.ItineraryDays.RemoveRange(tour.ItineraryDays);
                // _context.TourCategories.RemoveRange(tour.TourCategories);
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync() // Cần method này
        {
            return await _context.Categories.OrderBy(c => c.Name).AsNoTracking().ToListAsync();
        }


        public async Task<Tour?> GetTourByIdAsync(int tourId)
        {
            return await _context.Tours.FindAsync(tourId);
        }

        public async Task<Tour?> GetTourDetailsByIdAsync(int tourId)
        {
            return await _context.Tours
                                 .Include(t => t.TourCategories).ThenInclude(tc => tc.Category)
                                 .Include(t => t.ItineraryDays.OrderBy(i => i.DayNumber).ThenBy(i => i.Order)) // Sắp xếp lịch trình
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(t => t.TourID == tourId);
        }

        public async Task<Tour?> GetTourForEditByIdAsync(int tourId)
        {
            return await _context.Tours
                                .Include(t => t.TourCategories) // Chỉ cần ID category liên quan
                                .Include(t => t.ItineraryDays.OrderBy(i => i.DayNumber).ThenBy(i => i.Order))
                                .FirstOrDefaultAsync(t => t.TourID == tourId);
        }

        public async Task<(IEnumerable<Tour> Tours, int TotalCount)> GetToursPagedAsync(int pageIndex, int pageSize, string? searchTerm, string? statusFilter)
        {
            var query = _context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(t => t.Status == statusFilter);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(t => (t.Name != null && t.Name.ToLower().Contains(searchTerm)) ||
                                         (t.Destination != null && t.Destination.ToLower().Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();
            var tours = await query.OrderByDescending(t => t.TourID) // Sắp xếp mới nhất trước
                                   .Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync();
            return (tours, totalCount);
        }

        public async Task<bool> IsTourInUseAsync(int tourId)
        {
            // Kiểm tra xem có Booking hoặc TourGroup nào liên kết không
            bool hasBookings = await _context.Bookings.AnyAsync(b => b.TourID == tourId);
            if (hasBookings) return true;

            bool hasGroups = await _context.TourGroups.AnyAsync(tg => tg.TourID == tourId);
            return hasGroups;
        }

        public async Task UpdateItineraryAsync(int tourId, List<ItineraryDay> currentItinerary)
        {
            // Xóa itinerary cũ
            var oldItinerary = await _context.ItineraryDays.Where(i => i.TourID == tourId).ToListAsync();
            if (oldItinerary.Any())
            {
                _context.ItineraryDays.RemoveRange(oldItinerary);
            }

            // Thêm itinerary mới (đã được gán TourID ở Service)
            if (currentItinerary.Any())
            {
                await _context.ItineraryDays.AddRangeAsync(currentItinerary);
            }
            // SaveChanges sẽ được gọi bên ngoài trong transaction của Service
        }

        public async Task UpdateTourAsync(Tour tour)
        {
            _context.Entry(tour).State = EntityState.Modified;
            // SaveChanges sẽ được gọi bên ngoài trong transaction của Service
            await Task.CompletedTask; // Chỉ để hàm là async, không cần làm gì thêm ở đây
        }

        public async Task UpdateTourCategoriesAsync(int tourId, List<int> selectedCategoryIds)
        {
            var existingLinks = await _context.TourCategories.Where(tc => tc.TourID == tourId).ToListAsync();
            _context.TourCategories.RemoveRange(existingLinks); // Xóa hết link cũ

            if (selectedCategoryIds != null && selectedCategoryIds.Any())
            {
                var newLinks = selectedCategoryIds.Select(catId => new TourCategory { TourID = tourId, CategoryID = catId });
                await _context.TourCategories.AddRangeAsync(newLinks); // Thêm link mới
            }
            // SaveChanges sẽ được gọi bên ngoài trong transaction của Service
        }
    }
}

