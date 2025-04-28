
namespace Tourest.ViewModels.Tour
{
    public class TourGuideListViewModel
    {
        public int TourGuideUserID { get; set; }

        // Thuộc tính từ bảng Users
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }

        // Thuộc tính từ bảng TourGuides
        public string? ExperienceLevel { get; set; }
        public string? Specializations { get; set; }
        public string? ActiveStatus { get; set; }  

    }

}
