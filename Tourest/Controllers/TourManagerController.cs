using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.Data.Entities;
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

        public List<TourGuideListViewModel> TourGuides { get; set; }
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


        public async Task<IActionResult> GetUnassignedBookings()
        {
            try
            {

                var unassignedBookings = await _context.Bookings
                    .Where(b => b.TourGroupID != null)
                    .Include(b => b.Tour)
                    .Include(b => b.Customer)
                    .Join(_context.TourGroups,
                        booking => booking.TourGroupID,
                        tourGroup => tourGroup.TourGroupID,
                        (booking, tourGroup) => new { Booking = booking, TourGroup = tourGroup })
                    .Where(joined => joined.TourGroup.AssignedTourGuideID == null)
                    .Select(joined => joined.Booking)
                    .AsNoTracking()
                    .ToListAsync();


                Console.WriteLine($"Found {unassignedBookings.Count} unassigned bookings");


                return View("GetUnassignedBookings", unassignedBookings ?? new List<Booking>());
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in GetUnassignedBookings: {ex}");


                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
