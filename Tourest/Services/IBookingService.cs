using Tourest.ViewModels;
using Tourest.ViewModels.Booking;

namespace Tourest.Services
{
    public interface IBookingService
    {
        Task<BookingHistoryViewModel> GetBookingHistoryAsync(int customerId, string? destinationFilter = null);
        Task<(bool Success, string ErrorMessage, int? BookingId)> CreateBookingAsync(CreateBookingViewModel model, int customerId);
    }
}
