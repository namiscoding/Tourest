using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tourest.TourGuide.Services;

namespace Tourest.TourGuide.Controllers
{
    public class TourGuideRating : Controller
    {
        private readonly ITourAssignmentService _tourAssignmentService;

        public TourGuideRating(ITourAssignmentService tourAssignmentService)
        {
            _tourAssignmentService = tourAssignmentService;
        }
        public async Task<IActionResult> Index()
        {
            var tourGuideRating = await _tourAssignmentService.GetTourGuideRatingsAndComments(3, 1);
            return View(tourGuideRating);
        }
    }
}
