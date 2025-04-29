namespace Tourest.ViewModels.Admin.AdminDashboard
{
    public class AdminDashboardViewModel
    {
        // Số liệu tổng quan
        public int TotalCustomers { get; set; }
        public int TotalTourGuides { get; set; }
        public int TotalTourManagers { get; set; }
        public int TotalActiveTours { get; set; }
        public int TotalBookingsLast30Days { get; set; }
        public int TotalRevenueLast30Days { get; set; } // INT

        // Danh sách Top
        public List<TopTourViewModel> TopSellingToursByBooking { get; set; } = new List<TopTourViewModel>();
        public List<TopTourViewModel> TopSellingToursByRevenue { get; set; } = new List<TopTourViewModel>();
        public List<TopGuideViewModel> TopRatedGuides { get; set; } = new List<TopGuideViewModel>();


        // Dữ liệu biểu đồ
        public ChartDataViewModel? RevenueLast6Months { get; set; }
        public ChartDataViewModel? BookingsLast7Days { get; set; }

        public int PendingAssignments { get; set; } 
        public int OpenSupportRequests { get; set; }

        
    }
}
