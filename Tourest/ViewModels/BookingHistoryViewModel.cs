using Tourest.ViewModels.Booking;

namespace Tourest.ViewModels
{
    public class BookingHistoryViewModel
    {
        // Danh sách các booking item (đã lọc nếu có)
        public List<BookingHistoryItemViewModel> Bookings { get; set; } = new List<BookingHistoryItemViewModel>();

        // Danh sách các điểm đến duy nhất mà người dùng đã đặt để tạo dropdown filter
        public List<string> AvailableDestinations { get; set; } = new List<string>();

        // Lưu trữ filter người dùng đã chọn
        public string? SelectedDestinationFilter { get; set; }

        public int TotalSpentConfirmedCompleted { get; set; } 
        public List<MonthlySpendingViewModel> MonthlySpendingData { get; set; } = new List<MonthlySpendingViewModel>(); 
    }
}
