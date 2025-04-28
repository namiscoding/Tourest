using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin.AdminTour
{
    public class AdminItineraryDayViewModel
    {
        public int ItineraryDayID { get; set; } // = 0 nếu là ngày mới

        [Required(ErrorMessage = "Nhập số ngày.")]
        [Range(1, 100, ErrorMessage = "Số ngày không hợp lệ.")]
        [Display(Name = "Ngày số")]
        public int DayNumber { get; set; } = 1;

        [Required(ErrorMessage = "Nhập tiêu đề ngày.")]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; set; }

        public int Order { get; set; } // Tạm thời có thể không cần hiển thị/sửa trực tiếp
    }
}
