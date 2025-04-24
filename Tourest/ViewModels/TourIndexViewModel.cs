using Tourest.ViewModels.Category;
using Tourest.ViewModels.Tour;
using System.Collections.Generic;
namespace Tourest.ViewModels
{
    public class TourIndexViewModel
    {
        public IEnumerable<TourListViewModel> Tours { get; set; } = new List<TourListViewModel>();
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public List<int>? SelectedCategoryIds { get; set; } = new List<int>();

        public IEnumerable<string> AvailableDestinations { get; set; } = new List<string>();
        public List<string>? SelectedDestinations { get; set; } = new List<string>();

        public List<int>? SelectedRatings { get; set; } = new List<int>();

        public int? SelectedMinPrice { get; set; }
        public int? SelectedMaxPrice { get; set; }

        public string? CurrentSearchDestination { get; set; }
        public string? CurrentSearchCategoryName { get; set; }
        public DateTime? CurrentSearchDate { get; set; }
        public int? CurrentSearchGuests { get; set; }
    }
}
