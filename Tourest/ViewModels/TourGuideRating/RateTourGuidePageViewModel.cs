using Tourest.ViewModels.UserView;
namespace Tourest.ViewModels.TourGuideRating
{
    public class RateTourGuidePageViewModel
    {
        public UserViewModel TourGuideInfo { get; set; } = null!;

        // Dữ liệu và validation cho form đánh giá
        public CreateTourGuideRatingViewModel RatingForm { get; set; } = null!;
    }
}
