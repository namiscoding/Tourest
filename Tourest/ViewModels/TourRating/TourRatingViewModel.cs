using Tourest.ViewModels.UserView;
namespace Tourest.ViewModels.TourRating
    
{
    public class TourRatingViewModel
    {
        public int RatingId { get; set; }
        public decimal RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime RatingDate { get; set; }
        public UserViewModel? Customer { get; set; }
    }
}
