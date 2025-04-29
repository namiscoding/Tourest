using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Tourest.Data;
using Tourest.TourGuide.Services;

namespace Tourest.TourGuide.Controllers
{
    [Authorize(Roles = "TourGuide")]
    public class TourGuideRating : Controller
    {
       
        private readonly ITourAssignmentService _tourAssignmentService;
        private readonly ApplicationDbContext _context;
        public TourGuideRating(ITourAssignmentService tourAssignmentService, ApplicationDbContext context)
        {
            _tourAssignmentService = tourAssignmentService;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var tourGuideId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (tourGuideId == 0)
            {
                return Unauthorized("Tour guide not authenticated.");
            }

            // Pass the dynamic tourGuideId to the service method
            var tourGuideRating = await _tourAssignmentService.GetTourGuideRatingsAndComments(tourGuideId, null);
            return View(tourGuideRating);
        }
     
    }
}
