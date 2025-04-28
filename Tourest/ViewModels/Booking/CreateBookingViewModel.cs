using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Booking
{
    public class CreateBookingViewModel
    {
        [Required]
        public int TourId { get; set; } // Lấy từ input ẩn name="TourId"

        [Required(ErrorMessage = "Vui lòng chọn ngày khởi hành.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khởi hành")]
        public DateTime SelectedDate { get; set; } // Lấy từ input ẩn name="SelectedDate"

        [Required(ErrorMessage = "Vui lòng chọn điểm đón.")] // Bắt buộc nếu dropdown có required
        [Display(Name = "Điểm đón")]
        public string SelectedPickupPoint { get; set; } = string.Empty; // Lấy từ select name="SelectedPickupPoint"

        [Required]
        [Range(1, 100, ErrorMessage = "Số người lớn phải ít nhất là 1.")]
        [Display(Name = "Số người lớn")]
        public int NumberOfAdults { get; set; } // Lấy từ input name="NumberOfAdults"

        [Required]
        [Range(0, 100, ErrorMessage = "Số trẻ em không được âm.")]
        [Display(Name = "Số trẻ em")]
        public int NumberOfChildren { get; set; }
    }
}
