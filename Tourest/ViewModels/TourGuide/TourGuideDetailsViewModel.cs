namespace Tourest.ViewModels.TourGuide
{
    public class TourGuideDetailsViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ExperienceLevel { get; set; }
        public List<string> LanguagesSpokenList { get; set; } = new List<string>();
        public List<string> SpecializationsList { get; set; } = new List<string>();
        public decimal? AverageRating { get; set; } // Lấy từ TourGuide entity
        public int RatingCount { get; set; } // Tổng số lượt đánh giá

        public List<TourGuideRatingViewModel> CustomerRatings { get; set; } = new List<TourGuideRatingViewModel>();
    }
}
