namespace Tourest.ViewModels.Admin.AdminDashboard
{
    public class ChartDataViewModel
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Data { get; set; } = new List<int>(); // Hoặc decimal cho doanh thu
        public string? LabelName { get; set; } // Tên của bộ dữ liệu
    }
}
