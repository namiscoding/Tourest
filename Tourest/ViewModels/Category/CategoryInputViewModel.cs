using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Category
{
    public class CategoryInputViewModel
    {
        public int CategoryID { get; set; } // Cần cho Edit

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên Danh mục")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
