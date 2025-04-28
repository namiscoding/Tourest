using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels;
using Tourest.ViewModels.Tour;
namespace Tourest.Controllers
{
    public class ToursController : Controller
    {
        private readonly ITourService _tourService;
        private readonly ICategoryService _categoryService;
        // Inject thêm các service khác nếu cần

        public ToursController(ITourService tourService, ICategoryService categoryService)
        {
            _tourService = tourService;
            _categoryService = categoryService;
        }

        // GET: /Tours hoặc /Tours/Index?categoryIds=1&destinations=Hanoi&ratings=4&minPrice=1000000&sortBy=price_asc
        public async Task<IActionResult> Index(
            List<int>? categoryIds,
            List<string>? destinations,
            List<int>? ratings,
            string? sortBy,
            int? minPrice, // Tham số lọc giá min
            int? maxPrice,
            string? searchDestination,
            string? searchCategoryName,
            DateTime? searchDate,
            int? searchGuests) // Tham số lọc giá max
        {
            var categories = await _categoryService.GetAllCategoriesForDisplayAsync();
            var availableDestinations = await _tourService.GetDestinationsForFilterAsync();

            // === SỬA LỖI: Truyền minPrice và maxPrice xuống Service ===
            var toursResult = await _tourService.GetActiveToursForDisplayAsync(
                categoryIds, destinations, ratings, sortBy, minPrice, maxPrice,
                // --- TRUYỀN THAM SỐ TÌM KIẾM ---
                searchDestination, searchCategoryName, searchDate, searchGuests
            );

            // === KẾT THÚC SỬA LỖI ===

            var viewModel = new TourIndexViewModel
            {
                Tours = toursResult,
                Categories = categories,
                AvailableDestinations = availableDestinations,
                SelectedCategoryIds = categoryIds ?? new List<int>(),
                SelectedDestinations = destinations ?? new List<string>(),
                SelectedRatings = ratings ?? new List<int>(),
                SelectedMinPrice = minPrice, // Gán giá trị để giữ trạng thái slider
                SelectedMaxPrice = maxPrice  // Gán giá trị để giữ trạng thái slider
            };

            ViewBag.CurrentSortBy = sortBy; // Giữ lại để set selected cho dropdown

            return View(viewModel);
        }

        // GET: /Tours/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Tour ID.");
            }

            var tourDetailsViewModel = await _tourService.GetTourDetailsAsync(id);

            if (tourDetailsViewModel == null)
            {
                return NotFound();
            }

            // Truyền TourDetailsViewModel sang View "Details"
            // Đảm bảo bạn đã tạo Views/Tours/Details.cshtml và model của nó là TourDetailsViewModel
            return View("Details", tourDetailsViewModel); // Chỉ định rõ tên View nếu cần
        }

    }
}
