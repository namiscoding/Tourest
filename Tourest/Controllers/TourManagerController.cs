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
        private readonly IEmailService _emailService;
        public TourManagerController(ApplicationDbContext context, ITourManagerService tourManagerService, IEmailService emailService)
        {
            _context = context;
            _tourManagerService = tourManagerService;
            _emailService = emailService;
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
        [HttpGet("GetRejectedAssignedTours")]
        public async Task<IActionResult> GetRejectedAssignedTours()
        {
            try
            {
                var rejectedTours = await _context.TourGroups
                    .Where(tg => tg.AssignedTourGuideID != null) // Ensure the tour has been assigned
                    .Join(_context.TourGuideAssignments,
                        tg => tg.TourGroupID,
                        tga => tga.TourGroupID,
                        (tg, tga) => new { TourGroup = tg, TourGuideAssignment = tga })
                    .Where(joined => joined.TourGuideAssignment.Status == "Rejected") // Filter for rejected assignments
                    .Join(_context.Tours,
                        joined => joined.TourGroup.TourID,
                        tour => tour.TourID,
                        (joined, tour) => new { joined.TourGroup, joined.TourGuideAssignment, Tour = tour })
                    .Join(_context.TourGuides,
                        joined => joined.TourGroup.AssignedTourGuideID,
                        tg => tg.TourGuideUserID,
                        (joined, tourGuide) => new { joined.TourGroup, joined.TourGuideAssignment, joined.Tour, TourGuide = tourGuide })
                    .Join(_context.Users,
                        joined => joined.TourGuide.TourGuideUserID,
                        user => user.UserID,
                        (joined, user) => new { joined.TourGroup, joined.TourGuideAssignment, joined.Tour, TourGuideName = user.FullName })
                    .Select(x => new RejectedAssignedTourViewModel
                    {
                        TourGroupId = x.TourGroup.TourGroupID,
                        TourId = x.TourGroup.TourID,
                        TourName = x.Tour.Name,
                        AssignedTourGuideId = x.TourGroup.AssignedTourGuideID ?? 0,
                        TourGuideName = x.TourGuideName ?? "Unknown",
                        DepartureDate = x.TourGroup.DepartureDate,
                        Status = x.TourGuideAssignment.Status,
                        RejectionReason = x.TourGuideAssignment.RejectionReason
                    })
                    .AsNoTracking()
                    .ToListAsync();

                // Logging for debugging
              
                foreach (var tour in rejectedTours)
                {
                    Console.WriteLine($"TourGroupID: {tour.TourGroupId}, TourID: {tour.TourId}, TourName: {tour.TourName}");
                    Console.WriteLine($"AssignedTourGuideID: {tour.AssignedTourGuideId}, TourGuideName: {tour.TourGuideName}");
                    Console.WriteLine($"DepartureDate: {tour.DepartureDate}, Status: {tour.Status}, RejectionReason: {(tour.RejectionReason ?? "N/A")}");
                }

                return View("GetRejectedAssignedTours", rejectedTours);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRejectedAssignedTours: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost("AssignGuide")]
        public async Task<IActionResult> AssignGuide([FromBody] AssignGuideRequest request)
        {
            try
            {
                Console.WriteLine($"Assigning guide ID: {request.GuideId} to TourGroup ID: {request.TourGroupId}");

                if (request.TourGroupId <= 0 || request.GuideId <= 0)
                {
                    return Json(new { success = false, message = "Invalid tour group ID or guide ID" });
                }

                // Check TourGroups for a valid, unassigned tour group
                var tourGroup = await _context.TourGroups
                    .FirstOrDefaultAsync(tg => tg.TourGroupID == request.TourGroupId && tg.Status == "PendingAssignment");
                if (tourGroup == null)
                {
                    return Json(new { success = false, message = "Tour group not found or already assigned" });
                }

                // Verify the tour guide exists
                var guide = await _context.TourGuides
                    .FirstOrDefaultAsync(g => g.TourGuideUserID == request.GuideId);
                if (guide == null)
                {
                    return Json(new { success = false, message = "Tour guide not found" });
                }

                // Check for conflicting assignments on the same date
                var conflictingAssignment = await _context.TourGuideAssignments
                    .Join(_context.TourGroups,
                          tga => tga.TourGroupID,
                          tg => tg.TourGroupID,
                          (tga, tg) => new { tga, tg })
                    .Where(x => x.tga.TourGuideID == request.GuideId
                                && x.tga.Status == "Confirmed"
                                && x.tg.DepartureDate.Date == tourGroup.DepartureDate.Date)
                    .AnyAsync();
                if (conflictingAssignment)
                {
                    return Json(new { success = false, message = "Guide is already assigned to another tour on the same date" });
                }

                // Create a new tour guide assignment
                var assignment = new TourGuideAssignment
                {
                    TourGroupID = tourGroup.TourGroupID,
                    TourGuideID = request.GuideId,
                    TourManagerID = 2, // Replace with logged-in manager ID (e.g., from User.Identity)
                    AssignmentDate = DateTime.Now,
                    Status = "Pending"
                };

                // Update tour group status and assigned guide
                _context.TourGuideAssignments.Add(assignment);
                tourGroup.Status = "Assigned";
                tourGroup.AssignedTourGuideID = request.GuideId;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Guide assigned successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning guide: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                return Json(new { success = false, message = $"Error assigning guide: {ex.Message}" });
            }
        }
        [HttpPost("ReassignGuide")]
        public async Task<IActionResult> ReassignGuide([FromBody] ReassignGuideRequest request)
        {
            try
            {
                Console.WriteLine($"Reassigning guide ID: {request.GuideId} to TourGroup ID: {request.TourGroupId}");

                if (request.TourGroupId <= 0 || request.GuideId <= 0)
                {
                    return Json(new { success = false, message = "Invalid tour group ID or guide ID" });
                }

                // Fetch the tour group
                var tourGroup = await _context.TourGroups
                    .FirstOrDefaultAsync(tg => tg.TourGroupID == request.TourGroupId);
                if (tourGroup == null)
                {
                    return Json(new { success = false, message = "Tour group not found" });
                }

                // Check if there’s a rejected assignment for this tour group
                var existingAssignment = await _context.TourGuideAssignments
                    .FirstOrDefaultAsync(tga => tga.TourGroupID == request.TourGroupId && tga.Status == "Rejected");
                if (existingAssignment == null)
                {
                    return Json(new { success = false, message = "No rejected assignment found for this tour group" });
                }

                // Validate the guide
                var guide = await _context.TourGuides
                    .FirstOrDefaultAsync(g => g.TourGuideUserID == request.GuideId);
                if (guide == null)
                {
                    return Json(new { success = false, message = "Tour guide not found" });
                }

                // Check for conflicting assignments
                var conflictingAssignment = await _context.TourGuideAssignments
                    .Join(_context.TourGroups,
                          tga => tga.TourGroupID,
                          tg => tg.TourGroupID,
                          (tga, tg) => new { tga, tg })
                    .Where(x => x.tga.TourGuideID == request.GuideId
                                && x.tga.Status == "Confirmed"
                                && x.tg.DepartureDate.Date == tourGroup.DepartureDate.Date)
                    .AnyAsync();
                if (conflictingAssignment)
                {
                    return Json(new { success = false, message = "Guide is already assigned to another tour on the same date" });
                }

                // Update the existing assignment or create a new one
                existingAssignment.TourGuideID = request.GuideId;
                existingAssignment.TourManagerID = 2; // Replace with logged-in manager ID (e.g., User.Identity)
                existingAssignment.AssignmentDate = DateTime.Now;
                existingAssignment.Status = "Pending";
                existingAssignment.RejectionReason = null;

                // Update the tour group
                tourGroup.Status = "Assigned";
                tourGroup.AssignedTourGuideID = request.GuideId;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Guide reassigned successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reassigning guide: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                return Json(new { success = false, message = $"Error reassigning guide: {ex.Message}" });
            }
        }

        private async Task SendEmailToCustomersForGuideAssignment(
            List<Booking> bookings,
            string tourName,
            DateTime departureDate,
            string guideName,
            bool isReassignment)
        {
            foreach (var booking in bookings)
            {
                var customer = booking.Customer;
                var customerEmail = customer.Email;
                var customerName = customer.FullName ?? "Customer";

                var emailSubject = isReassignment
                    ? "Tour Guide Reassigned for Your Booking - Tourest"
                    : "Tour Guide Assigned for Your Booking - Tourest";

                var emailMessage = $@"
                    <h3>Tour Guide {(isReassignment ? "Reassigned" : "Assigned")}</h3>
                    <p>Dear {customerName},</p>
                    <p>We {(isReassignment ? "wanted to inform you that a new tour guide has been reassigned" : "are pleased to inform you that a tour guide has been assigned")} to your booking:</p>
                    <ul>
                        <li><strong>Tour Name:</strong> {tourName}</li>
                        <li><strong>Departure Date:</strong> {departureDate:MMM dd, yyyy}</li>
                        <li><strong>Assigned Guide:</strong> {guideName}</li>
                        <li><strong>Pickup Point:</strong> {booking.PickupPoint}</li>
                    </ul>
                    <p>We look forward to providing you with an amazing tour experience!</p>
                    <p>Best regards,<br/>Tourest Team</p>";

                bool emailSent = _emailService.SendEmail(customerEmail, emailSubject, emailMessage);
                if (!emailSent)
                {
                    Console.WriteLine($"Failed to send email to customer {customerEmail}, but {(isReassignment ? "reassignment" : "assignment")} was successful.");
                }

                // Small delay to avoid overwhelming the SMTP server (optional)
                await Task.Delay(100);
            }
        }
    }

}

public class AssignGuideRequest
{
    public int TourGroupId { get; set; }
    public int GuideId { get; set; }
}
public class ReassignGuideRequest
{
    public int TourGroupId { get; set; }
    public int GuideId { get; set; }
}