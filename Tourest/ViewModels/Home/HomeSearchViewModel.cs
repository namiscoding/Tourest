using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Home
{
    public class HomeSearchViewModel
    {
        [Display(Name = "Điểm đến")]
        public string? Destination { get; set; }

        [Display(Name = "Loại tour")]
        public int? CategoryId { get; set; } // Dùng ID để khớp với dropdown

        [Display(Name = "Ngày đi")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Display(Name = "Số khách")]
        [Range(1, int.MaxValue, ErrorMessage = "Số khách phải lớn hơn 0")]
        public int? Guests { get; set; }
    }
}
