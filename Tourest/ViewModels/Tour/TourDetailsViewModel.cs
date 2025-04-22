using Tourest.ViewModels.Category;
using Tourest.ViewModels.ItineraryDay;
using Tourest.ViewModels.TourRating;
namespace Tourest.ViewModels.Tour
{
    public class TourDetailsViewModel
    {
        public int TourId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationDays { get; set; }
        public int DurationNights { get; set; }
        public int AdultPrice { get; set; }
        public int ChildPrice { get; set; }
        public int? MinGroupSize { get; set; }
        public int? MaxGroupSize { get; set; }
        public List<string> DeparturePointsList { get; set; } = new List<string>(); // Tách chuỗi
        public List<string> IncludedServicesList { get; set; } = new List<string>(); // Tách chuỗi
        public List<string> ExcludedServicesList { get; set; } = new List<string>(); // Tách chuỗi
        public List<string> ImageUrlList { get; set; } = new List<string>(); // Tách chuỗi
        public string Status { get; set; } = string.Empty;
        public decimal? AverageRating { get; set; } // Sẽ được tính toán
        public int SumRating { get; set; } // Số lượng đánh giá
        public bool IsCancellable { get; set; }
        public string? CancellationPolicyDescription { get; set; }

        // Dữ liệu liên quan
        public List<ItineraryDayViewModel> ItineraryDays { get; set; } = new List<ItineraryDayViewModel>();
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public List<TourRatingViewModel> Ratings { get; set; } = new List<TourRatingViewModel>();
    }
}
