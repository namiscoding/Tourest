using Tourest.ViewModels.UserView;

namespace Tourest.ViewModels.TourGuide
{
    public class TourGuideRatingViewModel
    {
        public decimal RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime RatingDate { get; set; }
        public UserViewModel? Customer { get; set; }
    }
}
