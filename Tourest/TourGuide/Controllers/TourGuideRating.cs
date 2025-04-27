using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Tourest.Data;
using Tourest.TourGuide.Services;

namespace Tourest.TourGuide.Controllers
{
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
            var tourGuideRating = await _tourAssignmentService.GetTourGuideRatingsAndComments(3, null);
            return View(tourGuideRating);
        }
     
    }
}
