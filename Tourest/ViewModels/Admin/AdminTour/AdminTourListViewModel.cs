using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin.AdminTour
{
    public class AdminTourListViewModel
    {
        public int TourId { get; set; }

        [Display(Name = "Tên Tour")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Điểm đến")]
        public string Destination { get; set; } = string.Empty;

        [Display(Name = "Số ngày")]
        public int DurationDays { get; set; }

        [Display(Name = "Giá Người lớn")]
        [DisplayFormat(DataFormatString = "{0:N0}")] // Format số nguyên
        public int AdultPrice { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Ảnh bìa")]
        public string? ThumbnailUrl { get; set; } // URL ảnh đầu tiên đã tạo
    }
}
