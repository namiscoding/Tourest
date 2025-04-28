using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.TourGuideRating
{
    public class CreateTourGuideRatingViewModel
    {
        [Required]
        public int TourGuideId { get; set; } // UserID của Tour Guide - Input ẩn

        [Required]
        public int TourGroupId { get; set; } // Input ẩn

        public int? BookingId { get; set; } // Input ẩn (tùy chọn)

        // Thông tin hiển thị trên form
        public string TourGuideName { get; set; } = string.Empty;
        public string? TourName { get; set; } // Tên tour liên quan (tùy chọn)
        public DateTime? DepartureDate { get; set; } // Ngày đi (tùy chọn)

        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá.")]
        [Range(1, 5, ErrorMessage = "Vui lòng chọn từ 1 đến 5 sao.")]
        [Display(Name = "Đánh giá của bạn cho Hướng dẫn viên")]
        public decimal RatingValue { get; set; }

        [StringLength(1000)]
        [Display(Name = "Viết bình luận (tùy chọn)")]
        [DataType(DataType.MultilineText)]
        public string? Comment { get; set; }
    }
}
