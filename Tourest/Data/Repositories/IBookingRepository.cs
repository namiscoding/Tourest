using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingsByCustomerIdAsync(int customerId);
        Task AddAsync(Booking booking);
        Task<Booking?> GetByIdWithDetailsAsync(int bookingId);

        // Task<Booking?> GetBookingByIdAsync(int bookingId);
    }
}
