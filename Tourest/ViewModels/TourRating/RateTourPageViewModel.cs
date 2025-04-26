using Tourest.ViewModels.Tour;

namespace Tourest.ViewModels.TourRating
{
    public class RateTourPageViewModel
    {
        public TourDetailsViewModel TourDetails { get; set; } = null!;
        public CreateTourRatingViewModel RatingForm { get; set; } = null!;
    }
}
