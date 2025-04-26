using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.ViewModels;
using Tourest.ViewModels.Tour;

namespace Tourest.Controllers
{
    [Route("TourManager")]
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
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /TourManager/TourGuide
        [HttpGet("TourGuide")]
        public IActionResult TourGuide()
        {
            var tourGuides = _tourManagerService.GetAllTourGuides();
            return View("TourGuide", tourGuides);
        }

        // GET: /TourManager/IndexDetail
        [HttpGet("IndexDetail")]
        public IActionResult IndexDetail()
        {
            return View();
        }

        // GET: /TourManager/TourGuideDetail/5
        [HttpGet("TourGuideDetail/{id}")]
        public IActionResult TourGuideDetail(int id)
        {
            var tourGuide = _tourManagerService.GetDetail(id);
            if (tourGuide == null)
            {
                return NotFound();
            }
            return View("TourGuideDetail", tourGuide);
        }

        // GET: /TourManager/TourGuideFeedback/5
        [HttpGet("TourGuideFeedback/{id}")]
        public async Task<IActionResult> TourGuideFeedback(int id)
        {
            var feedbacks = await _tourManagerService.GetFeedbacksByTourGuideIdAsync(id);

            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound();
            }

            return View(feedbacks);
        }

        // GET: /TourManager/ViewCustomers/10
        [HttpGet("ViewCustomers/{tourId}")]
        public async Task<IActionResult> ViewCustomers(int tourId)
        {
            var customers = await _tourManagerService.GetCustomersForTourAsync(tourId);
            return View(customers);
        }

        // GET: /TourManager/ListTour
        [HttpGet("ListTour")]
        public IActionResult ListTour()
        {
            var tours = _tourManagerService.GetAllTours();
            return View(tours);
        }

        // GET: /TourManager/GetUnassignedBookings
        [HttpGet("GetUnassignedBookings")]
        public async Task<IActionResult> GetUnassignedBookings()
        {
            try
            {
                var bookings = await _context.Bookings
                    .Where(b => b.TourGroupID != null)
                    .Include(b => b.Tour)
                    .Include(b => b.Customer)
                    .Join(_context.TourGroups,
                        booking => booking.TourGroupID,
                        tourGroup => tourGroup.TourGroupID,
                        (booking, tourGroup) => new { Booking = booking, TourGroup = tourGroup })
                    .Where(joined => joined.TourGroup.AssignedTourGuideID == null)
                    .Select(joined => new UnassignedBookingViewModel
                    {
                        BookingId = joined.Booking.BookingID,
                        TourGroupId = joined.TourGroup.TourGroupID,
                        TourName = joined.Booking.Tour.Name,
                        BookingDate = joined.Booking.BookingDate,
                        DepartureDate = joined.Booking.DepartureDate,
                        NumberOfAdults = joined.Booking.NumberOfAdults,
                        NumberOfChildren = joined.Booking.NumberOfChildren,
                        PickupPoint = joined.Booking.PickupPoint,
                        TotalPrice = joined.Booking.TotalPrice,
                        Status = joined.Booking.Status,
                        TourId = joined.Booking.TourID
                    })
                    .AsNoTracking()
                    .ToListAsync();



                return View("GetUnassignedBookings", bookings);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("GetRecommendedGuides/{tourGroupId}")]
        public async Task<IActionResult> GetRecommendedGuides(int tourGroupId)
        {
            try
            {
                var tourGroup = await _context.TourGroups
                    .Include(tg => tg.Tour)
                    .FirstOrDefaultAsync(tg => tg.TourGroupID == tourGroupId);
                if (tourGroup == null) return NotFound("Tour group not found");

                var totalGuests = tourGroup.TotalGuests;
                var requiredExperience = CalculateRequiredExperience(totalGuests);
                var tourDifficulty = GetTourDifficulty(tourGroup.TourID);

                // Fetch raw data with SQL-compatible operations
                var guideData = await _context.TourGuides
                    .Include(tg => tg.User)
                    .Where(tg => tg.MaxGroupSizeCapacity != null
                        && tg.MaxGroupSizeCapacity >= totalGuests
                        && tg.User != null)
                    .Select(tg => new
                    {
                        tg.TourGuideUserID,
                        UserFullName = tg.User.FullName ?? "Unknown",
                        ExperienceLevel = tg.ExperienceLevel ?? "0",
                        LanguagesSpoken = tg.LanguagesSpoken ?? "",
                        AverageRating = (double)(tg.AverageRating ?? 0),
                        MaxGroupSizeCapacity = (int)(tg.MaxGroupSizeCapacity ?? 0),
                        Specializations = tg.Specializations ?? ""
                    })
                    .Take(10) 
                    .ToListAsync();

               
                var recommendedGuides = guideData
                    .Select(g => new RecommendedGuideViewModel
                    {
                        Id = g.TourGuideUserID,
                        Name = g.UserFullName,
                        Experience = ExtractYearsFromExperience(g.ExperienceLevel),
                        Languages = g.LanguagesSpoken,
                        Rating = g.AverageRating,
                        MaxCapacity = g.MaxGroupSizeCapacity,
                        Specializations = g.Specializations,
                        SuitabilityScore = (float)CalculateSuitabilityScore(
                            new Tourest.Data.Entities.TourGuide
                            {
                                TourGuideUserID = g.TourGuideUserID,
                                ExperienceLevel = g.ExperienceLevel,
                                AverageRating = (decimal?)(float?)g.AverageRating,
                                MaxGroupSizeCapacity = g.MaxGroupSizeCapacity,
                                Specializations = g.Specializations
                            },
                            totalGuests,
                            tourDifficulty),
                        TourDifficulty = tourDifficulty,
                        RequiredExperience = requiredExperience
                    })
                    .OrderByDescending(g => g.SuitabilityScore)
                    .ThenByDescending(g => g.Rating)
                    .Take(5)
                    .ToList();

                
                if (recommendedGuides == null || recommendedGuides.Count == 0)
                {
                    Console.WriteLine("No recommended guides found.");
                }
                else
                {
                    foreach (var guide in recommendedGuides)
                    {
                        Console.WriteLine("----- Recommended Guide -----");
                        Console.WriteLine($"ID: {guide.Id}");
                        Console.WriteLine($"Name: {guide.Name}");
                        Console.WriteLine($"Experience (years): {guide.Experience}");
                        Console.WriteLine($"Languages: {guide.Languages}");
                        Console.WriteLine($"Rating: {guide.Rating}");
                        Console.WriteLine($"Max Capacity: {guide.MaxCapacity}");
                        Console.WriteLine($"Specializations: {guide.Specializations}");
                        Console.WriteLine($"Suitability Score: {guide.SuitabilityScore}");
                    }
                }

                return Ok(recommendedGuides);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRecommendedGuides: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        private int CalculateRequiredExperience(int groupSize)
        {
            if (groupSize > 30) return 5;
            if (groupSize > 20) return 3;
            return 1;
        }

        private int GetTourDifficulty(int tourId)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.TourID == tourId);
            if (tour == null) return 3;

            int difficultyScore = 0;
            if (tour.DurationDays >= 5) difficultyScore += 2;
            else if (tour.DurationDays >= 3) difficultyScore += 1;

            var locationCount = _context.ItineraryDays.Count(id => id.TourID == tourId);
            if (locationCount > 5) difficultyScore += 2;
            else if (locationCount > 3) difficultyScore += 1;

            if (tour.Description.Contains("trekking") || tour.Description.Contains("leo núi"))
                difficultyScore += 2;
            if (tour.Description.Contains("dã ngoại") || tour.Description.Contains("cắm trại"))
                difficultyScore += 1;

            return Math.Clamp(difficultyScore, 1, 5);
        }

        private float CalculateSuitabilityScore(Tourest.Data.Entities.TourGuide guide, int groupSize, int tourDifficulty)
        {
            decimal score = 0;
            var expYears = (decimal)ExtractYearsFromExperience(guide.ExperienceLevel);
            score += Math.Min(expYears / 10 * 30, 30);
            score += (guide.AverageRating ?? 3) / 5 * 25;
            var capacityRatio = (float)groupSize / guide.MaxGroupSizeCapacity;
            score += (1 - Math.Min((decimal)capacityRatio, 1)) * 25;

            if (guide.Specializations.Contains("Văn hóa")) score += 10;
            if (guide.Specializations.Contains("Ẩm thực")) score += 10;

            return (float)score;
        }

        private int ExtractYearsFromExperience(string experience)
        {
            var match = Regex.Match(experience, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }
    }
}
