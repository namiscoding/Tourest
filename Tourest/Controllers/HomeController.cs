using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels;
using Tourest.ViewModels.Home;
using Tourest.ViewModels.Tour;

namespace Tourest.Controllers
{
	public class HomeController : Controller
	{
        private readonly ITourService _tourService;
        private readonly IRatingService _ratingService;
        private readonly ICategoryService _categoryService;
        // Inject ILogger nếu cần

        public HomeController(ITourService tourService, IRatingService ratingService, ICategoryService categoryService)
        {
            _tourService = tourService;
            _ratingService = ratingService;
            _categoryService = categoryService;
        }

        // GET: / or /Home/Index
        public async Task<IActionResult> Index()
        {
            int tourCount = await _tourService.GetActiveTourCountAsync();
            int destinationCount = await _tourService.GetDistinctDestinationCountAsync();
            int reviewCount = await _ratingService.GetTotalTourRatingCountAsync();
            var categoryStats = await _categoryService.GetCategoryStatisticsAsync(); // Trả về List<CategoryStatViewModel>
            var availableCategories = await _categoryService.GetAllCategoriesForDisplayAsync(); // Trả về List<CategoryViewModel>
            var featuredTours = await _tourService.GetFeaturedToursForDisplayAsync(6); // Ví dụ lấy 6 tour

            var viewModel = new HomeViewModel
            {
                TotalActiveTourCount = tourCount,
                TotalDestinationCount = destinationCount,
                TotalReviewCount = reviewCount,
                CategoryStats = categoryStats ?? new List<ViewModels.Category.CategoryStatViewModel>(),
                AvailableCategories = (List<ViewModels.Category.CategoryViewModel>)availableCategories,
                SearchForm = new HomeSearchViewModel(),
                FeaturedTours = featuredTours ?? new List<TourListViewModel>() // Gán kết quả vào ViewModel
            };

            return View(viewModel);
        }
    }
}
