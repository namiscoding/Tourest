using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(ApplicationDbContext context, ILogger<BookingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerID == customerId)
                .Include(b => b.Tour)
                .Include(b => b.TourGroup)
                .OrderByDescending(b => b.BookingDate) // Sắp xếp theo ngày đặt mới nhất
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdWithDetailsAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.TourGroup) // Include TourGroup
                    .ThenInclude(tg => tg.AssignedTourGuide) // Include Guide từ Group
                .FirstOrDefaultAsync(b => b.BookingID == bookingId);
        }
        public async Task<int> GetBookingCountAsync(DateTime start, DateTime end, List<string>? validStatuses = null)
        {
            _logger.LogInformation("Getting booking count between {StartDate} and {EndDate}", start.ToShortDateString(), end.ToShortDateString());
            DateTime adjustedEndDate = end.Date.AddDays(1);
            validStatuses ??= new List<string> { "Paid", "Confirmed", "Completed" }; // Trạng thái mặc định hợp lệ

            try
            {
                return await _context.Bookings
                    .Where(b => validStatuses.Contains(b.Status) && b.BookingDate >= start.Date && b.BookingDate < adjustedEndDate)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking count.");
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetBookingsGroupedByDayAsync(DateTime start, DateTime end, List<string>? validStatuses = null)
        {
            _logger.LogInformation("Getting booking count grouped by day between {StartDate} and {EndDate}", start.ToShortDateString(), end.ToShortDateString());
            DateTime adjustedEndDate = end.Date.AddDays(1);
            validStatuses ??= new List<string> { "Paid", "Confirmed", "Completed" };

            try
            {
                var dailyCounts = await _context.Bookings
                    .Where(b => validStatuses.Contains(b.Status) && b.BookingDate >= start.Date && b.BookingDate < adjustedEndDate)
                    .GroupBy(b => b.BookingDate.Date) // Nhóm theo ngày (bỏ qua giờ)
                    .Select(g => new {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToDictionaryAsync(x => x.Date.ToString("yyyy-MM-dd"), x => x.Count); // Key là string "YYYY-MM-DD"

                // Điền các ngày không có booking bằng 0 (quan trọng cho biểu đồ liên tục)
                var filledCounts = new Dictionary<string, int>();
                for (var dt = start.Date; dt < adjustedEndDate; dt = dt.AddDays(1))
                {
                    var dateString = dt.ToString("yyyy-MM-dd");
                    filledCounts[dateString] = dailyCounts.GetValueOrDefault(dateString, 0);
                }
                return filledCounts;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings grouped by day.");
                return new Dictionary<string, int>();
            }
        }
    }
}
