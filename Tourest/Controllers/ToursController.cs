using Microsoft.AspNetCore.Mvc;
using Tourest.Services;

namespace Tourest.Controllers
{
    public class ToursController : Controller
    {
        private readonly ITourService _tourService;

        public ToursController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // GET: /Tours hoặc /Tours/Index
        public async Task<IActionResult> Index()
        {
            // 1. Gọi Service để lấy danh sách ViewModel phù hợp cho View
            var activeToursViewModel = await _tourService.GetActiveToursForDisplayAsync();

            // 2. Truyền danh sách ViewModel này sang View
            return View(activeToursViewModel); // Model của View là IEnumerable<TourItemViewModel>
        }
    }
}
