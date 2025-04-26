using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.TourRating
{
    public class CreateTourRatingViewModel
    {
        [Required]
        public int TourId { get; set; } // Input ẩn

        public int? BookingId { get; set; } // Input ẩn (tùy chọn, để tham chiếu)

        // Thuộc tính để hiển thị tên tour trên form cho người dùng biết đang đánh giá tour nào
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá.")]
        [Range(1, 5, ErrorMessage = "Vui lòng chọn từ 1 đến 5 sao.")]
        [Display(Name = "Đánh giá của bạn")]
        public decimal RatingValue { get; set; } // Dùng decimal để nhất quán với Entity

        [StringLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự.")]
        [Display(Name = "Viết bình luận (tùy chọn)")]
        [DataType(DataType.MultilineText)]
        public string? Comment { get; set; }
    }
}
