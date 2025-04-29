using Microsoft.AspNetCore.Mvc; // Đảm bảo using này tồn tại
using Tourest.Services;
using System.Threading.Tasks; // Thêm using cho Task

namespace Tourest.Controllers
{
    
    public class TourGuidesController : Controller
    {
        private readonly ITourGuideService _tourGuideService;

        public TourGuidesController(ITourGuideService tourGuideService) 
        {
            _tourGuideService = tourGuideService;
        }

        // GET: TourGuides/Details/5 (với 5 là UserID của TourGuide)
        public async Task<IActionResult> Details(int id)
        {
            id = 3; // Chỉ để test, bạn có thể xóa dòng này đi
            if (id <= 0)
            {
                return BadRequest();
            }

            var viewModel = await _tourGuideService.GetTourGuideDetailsAsync(id);

            if (viewModel == null)
            {
                return NotFound(); 
            }

           
            return View(viewModel); 
        }

    }
}