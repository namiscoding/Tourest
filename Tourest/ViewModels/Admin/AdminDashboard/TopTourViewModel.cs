using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin.AdminDashboard
{
    public class TopTourViewModel
    {
        public int TourId { get; set; }

        [Display(Name = "Tên Tour")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Điểm đến")]
        public string Destination { get; set; } = string.Empty; // << THÊM MỚI

        [Display(Name = "Số ngày")]
        public int DurationDays { get; set; } // << THÊM MỚI

        [Display(Name = "Số Bookings")]
        public int BookingCount { get; set; } // Giữ lại để dùng cho top tour theo booking

        [Display(Name = "Tổng Doanh thu")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalRevenue { get; set; } // << THÊM MỚI (Kiểu INT)

        [Display(Name = "Rating TB")]
        [DisplayFormat(DataFormatString = "{0:0.0}", NullDisplayText = "N/A")]
        public decimal? AverageRating { get; set; }

        // Giữ lại tên này theo yêu cầu của bạn, nó sẽ chứa PublicId
        public string? ThumbnailImagePublicId { get; set; }
    }
}
