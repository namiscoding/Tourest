using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerID == customerId)
                .Include(b => b.Tour) // Quan trọng: Lấy thông tin Tour liên quan
                .OrderByDescending(b => b.BookingDate) // Sắp xếp theo ngày đặt mới nhất
                .AsNoTracking()
                .ToListAsync();
        }

        
    }
}
