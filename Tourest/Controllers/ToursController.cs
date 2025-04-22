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

        public ToursController(ITourService tourService, ICategoryService categoryService)
        {
            _tourService = tourService;
            _categoryService = categoryService;
        }

        // GET: /Tours hoặc /Tours/Index?categoryIds=1&categoryIds=3...
        // Thay đổi tham số thành mảng hoặc list int
        public async Task<IActionResult> Index(
            List<int>? categoryIds,
            List<string>? destinations,
            List<int>? ratings,
            string? sortBy,
            int? minPrice, // THÊM
            int? maxPrice)
        {
            var categories = await _categoryService.GetAllCategoriesForDisplayAsync();
            var availableDestinations = await _tourService.GetDestinationsForFilterAsync();

            // Gọi service không có tham số phân trang
            var toursResult = await _tourService.GetActiveToursForDisplayAsync(
                categoryIds,
                destinations,
                ratings,
                sortBy);

            var viewModel = new TourIndexViewModel
            {
                // Gán kết quả vào Tours
                Tours = toursResult,

                Categories = categories,
                AvailableDestinations = availableDestinations,
                SelectedCategoryIds = categoryIds ?? new List<int>(),
                SelectedDestinations = destinations ?? new List<string>(),
                SelectedRatings = ratings ?? new List<int>()
            };

            // Giữ lại ViewBag cho sort nếu cần
            ViewBag.CurrentSortBy = sortBy;
            // Bỏ các ViewBag cho pagination

            return View(viewModel);
        }

        // GET: /Tours/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                // Có thể trả về trang lỗi hoặc trang NotFound chung
                return BadRequest("Invalid Tour ID.");
            }

            // Gọi service để lấy ViewModel chi tiết
            var tourDetailsViewModel = await _tourService.GetTourDetailsAsync(id);

            // Nếu không tìm thấy tour (service trả về null)
            if (tourDetailsViewModel == null)
            {
                // Trả về trang NotFound chuẩn của ASP.NET Core
                return NotFound();
            }

            // Truyền ViewModel sang View "Details"
            return View(tourDetailsViewModel);
        }

    }
}
