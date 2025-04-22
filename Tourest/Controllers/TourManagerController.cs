using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.Services;
using Tourest.ViewModels.Tour;

namespace Tourest.Controllers
{
    public class TourManagerController : Controller
    {
        private readonly ITourManagerService _tourManagerService;
        private readonly ApplicationDbContext _context;
        public TourManagerController(ApplicationDbContext context, ITourManagerService tourManagerService)
        {
            _context = context;
            _tourManagerService = tourManagerService;
        }

        public List<TourGuideListViewModel> TourGuides {  get; set; }
        // GET: /TourManager
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TourGuide()
        {
            var tourGuides = _tourManagerService.GetAllTourGuides(); 
            return View("TourGuide", tourGuides);
        }
        public IActionResult IndexDetail()
        {
            return View();
        }

        public IActionResult TourGuideDetail(int id)
        {
            var tourGuide = _tourManagerService.GetDetail(id);
            if (tourGuide == null)
            {
                return NotFound();
            }
            return View("TourGuideDetail", tourGuide);
        }
        public async Task<IActionResult> TourGuideFeedback(int id)
        {
            var feedbacks = await _tourManagerService.GetFeedbacksByTourGuideIdAsync(id);   

            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound();   
            }

            return View(feedbacks);   
        }
        public async Task<IActionResult> ViewCustomers(int tourId)
        {
            var customers = await _tourManagerService.GetCustomersForTourAsync(tourId);
            return View(customers);  
        }
        public IActionResult ListTour()
        {
            var tours = _tourManagerService.GetAllTours();
            return View(tours);
        }
    }
}
