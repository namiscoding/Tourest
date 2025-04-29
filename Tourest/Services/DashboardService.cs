using Tourest.Data.Repositories;
using Tourest.ViewModels.Admin.AdminDashboard;

namespace Tourest.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IBookingRepository _bookingRepository; 
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IUserRepository userRepository,
            ITourRepository tourRepository,
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository,
            ILogger<DashboardService> logger)
        {
            _userRepository = userRepository;
            _tourRepository = tourRepository;
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync(TimePeriod period = TimePeriod.Last30Days)
        {
            _logger.LogInformation("Generating dashboard data sequentially for period: {Period}", period);
            var viewModel = new AdminDashboardViewModel();

            // Xác định khoảng thời gian (Giữ nguyên)
            DateTime endDate = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1); // Hết ngày hôm nay UTC
            DateTime startDate;
            switch (period)
            {
                case TimePeriod.Last7Days: startDate = endDate.AddDays(-7).Date; break;
                case TimePeriod.CurrentMonth: startDate = new DateTime(endDate.Year, endDate.Month, 1); break;
                case TimePeriod.CurrentYear: startDate = new DateTime(endDate.Year, 1, 1); break;
                case TimePeriod.Last30Days:
                default: startDate = endDate.AddDays(-30).Date; break;
            }
            _logger.LogInformation("Date range for statistics: {StartDate} to {EndDate}", startDate, endDate);

            // --- Thực thi và gán kết quả tuần tự ---
            try
            {
                _logger.LogInformation("Fetching counts...");
                viewModel.TotalCustomers = await _userRepository.GetUserCountByRoleAsync("Customer");
                viewModel.TotalTourGuides = await _userRepository.GetUserCountByRoleAsync("TourGuide");
                viewModel.TotalTourManagers = await _userRepository.GetUserCountByRoleAsync("TourManager");

                var tourCounts = await _tourRepository.GetTourCountByStatusAsync();
                viewModel.TotalActiveTours = tourCounts.GetValueOrDefault("Active", 0);
                // Gán thêm các status khác nếu cần (ví dụ: viewModel.TotalDraftTours = tourCounts.GetValueOrDefault("Draft", 0);)

                List<string> bookingCountStatuses = new List<string> { "Paid", "Confirmed", "Completed" };
                viewModel.TotalBookingsLast30Days = await _bookingRepository.GetBookingCountAsync(startDate, endDate, bookingCountStatuses); // Dùng đúng period
                viewModel.TotalRevenueLast30Days = await _paymentRepository.GetTotalRevenueAsync(startDate, endDate); // Dùng đúng period
                _logger.LogInformation("Counts fetched successfully.");


                _logger.LogInformation("Fetching chart data...");
                // Dữ liệu cho biểu đồ doanh thu (6 tháng)
                var revenueData = await _paymentRepository.GetRevenueGroupedByMonthAsync(endDate.AddMonths(-5).AddDays(1).Date, endDate);
                viewModel.RevenueLast6Months = new ChartDataViewModel
                {
                    Labels = revenueData.Keys.ToList(),
                    Data = revenueData.Values.ToList(),
                    LabelName = "Doanh thu (VNĐ)"
                };

                // Dữ liệu cho biểu đồ booking (7 ngày)
                List<string> bookingChartStatuses = new List<string> { "Paid", "Confirmed", "Completed" };
                var bookingData = await _bookingRepository.GetBookingsGroupedByDayAsync(endDate.AddDays(-6).Date, endDate, bookingChartStatuses);
                viewModel.BookingsLast7Days = new ChartDataViewModel
                {
                    Labels = bookingData.Keys.ToList(),
                    Data = bookingData.Values.ToList(),
                    LabelName = "Số Booking"
                };
                _logger.LogInformation("Chart data fetched successfully.");


                _logger.LogInformation("Fetching top lists...");
                // Top Lists (Ví dụ - Lấy theo Doanh thu và Rating Guide)
                // Cần đảm bảo các phương thức Repo này trả về đúng kiểu ViewModel hoặc bạn cần map ở đây
                viewModel.TopSellingToursByRevenue = (await _tourRepository.GetTopSellingToursByRevenueAsync(5, startDate, endDate))?.ToList() ?? new List<TopTourViewModel>();
                viewModel.TopSellingToursByBooking = (await _tourRepository.GetTopSellingToursByBookingCountAsync(5, startDate, endDate))?.ToList() ?? new List<TopTourViewModel>();
                viewModel.TopRatedGuides = (await _userRepository.GetTopRatedGuidesAsync(5))?.ToList() ?? new List<TopGuideViewModel>();
                _logger.LogInformation("Top lists fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dashboard data.");
                
            }


            _logger.LogInformation("Finished generating dashboard data sequentially.");
            return viewModel;
        }
       
    }
}
