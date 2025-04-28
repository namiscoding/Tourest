using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.SupportRequest
{
    public class CreateSupportRequestViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự.")]
        [Display(Name = "Tiêu đề")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung yêu cầu.")]
        [StringLength(2000, ErrorMessage = "Nội dung không được vượt quá 2000 ký tự.")]
        [Display(Name = "Nội dung yêu cầu")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; } = string.Empty;
    }
}
