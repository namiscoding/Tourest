using Tourest.ViewModels.Category;
using Tourest.ViewModels.Tour;

namespace Tourest.ViewModels.Home
{
    public class HomeViewModel
    {
        public HomeSearchViewModel SearchForm { get; set; } = new HomeSearchViewModel();
        public List<CategoryViewModel> AvailableCategories { get; set; } = new List<CategoryViewModel>(); // Cho dropdown category

        // Dữ liệu thống kê
        public int TotalDestinationCount { get; set; }
        public int TotalReviewCount { get; set; }
        public int TotalActiveTourCount { get; set; }
        public List<CategoryStatViewModel> CategoryStats { get; set; } = new List<CategoryStatViewModel>();
        public List<TourListViewModel> FeaturedTours { get; set; } = new List<TourListViewModel>();
    }
}
