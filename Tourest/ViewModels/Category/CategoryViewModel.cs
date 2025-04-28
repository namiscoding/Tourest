using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Category
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        [Display(Name = "Tên Danh mục")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
