using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin.AdminTour
{
    public class CreateEditTourViewModel
    {
        public int TourID { get; set; } // Chỉ dùng cho Edit

        [Required(ErrorMessage = "Vui lòng nhập tên tour.")]
        [StringLength(200)]
        [Display(Name = "Tên Tour")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập điểm đến.")]
        [StringLength(255)]
        [Display(Name = "Điểm đến")]
        public string Destination { get; set; } = string.Empty;

        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Nhập số ngày.")]
        [Range(1, 365)]
        [Display(Name = "Số ngày")]
        public int DurationDays { get; set; } = 1;

        [Required(ErrorMessage = "Nhập số đêm.")]
        [Range(0, 364)]
        [Display(Name = "Số đêm")]
        public int DurationNights { get; set; } = 0;

        [Required(ErrorMessage = "Nhập giá người lớn.")]
        [Range(0, int.MaxValue)]
        [Display(Name = "Giá Người lớn (VNĐ)")]
        public int AdultPrice { get; set; }

        [Required(ErrorMessage = "Nhập giá trẻ em.")]
        [Range(0, int.MaxValue)]
        [Display(Name = "Giá Trẻ em (VNĐ)")]
        public int ChildPrice { get; set; }

        [Range(1, 200)]
        [Display(Name = "Số khách tối thiểu")]
        public int? MinGroupSize { get; set; }

        [Range(1, 500)]
        [Display(Name = "Số khách tối đa")]
        public int? MaxGroupSize { get; set; }

        [StringLength(500)]
        [Display(Name = "Điểm khởi hành (cách nhau bởi ;)")]
        public string? DeparturePoints { get; set; }

        [Display(Name = "Dịch vụ bao gồm")]
        public string? IncludedServices { get; set; }

        [Display(Name = "Dịch vụ không bao gồm")]
        public string? ExcludedServices { get; set; }

        [Required(ErrorMessage = "Chọn trạng thái.")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Draft"; // Mặc định là Draft

        [Display(Name = "Cho phép hủy?")]
        public bool IsCancellable { get; set; } = false;

        [Display(Name = "Chính sách hủy")]
        public string? CancellationPolicyDescription { get; set; }

        // --- Dữ liệu liên quan ---

        // Danh sách ID các Category được chọn từ form
        [Display(Name = "Danh mục")]
        public List<int> SelectedCategoryIds { get; set; } = new();

        // Danh sách tất cả Category để hiển thị lựa chọn (checkbox)
        public List<CategorySelectionViewModel> AvailableCategories { get; set; } = new();

        // Danh sách các ngày trong lịch trình (để JS xử lý trên form)
        public List<AdminItineraryDayViewModel> ItineraryDays { get; set; } = new();

        // Danh sách PublicId các ảnh hiện có (cho Edit View)
        public List<string> ExistingImagePublicIds { get; set; } = new();

        // Input nhận các file ảnh mới upload lên
        [Display(Name = "Ảnh Mới")]
        public List<IFormFile>? NewImageFiles { get; set; }

        // Input (thường là hidden, được JS cập nhật) chứa các PublicId cần xóa
        public List<string>? ImagesToDeletePublicIds { get; set; }
    }
}
