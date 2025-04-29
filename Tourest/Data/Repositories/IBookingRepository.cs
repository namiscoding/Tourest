using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingsByCustomerIdAsync(int customerId);
        Task AddAsync(Booking booking);
        Task<Booking?> GetByIdWithDetailsAsync(int bookingId);

        Task<int> GetBookingCountAsync(DateTime start, DateTime end, List<string>? validStatuses);
        Task<Dictionary<string, int>> GetBookingsGroupedByDayAsync(DateTime start, DateTime end, List<string>? validStatuses); // Trả về Dictionary<DateString, Count>


        // Task<Booking?> GetBookingByIdAsync(int bookingId);
    }
}
