using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tourest.TourGuide.Repositories;
using Tourest.TourGuide.Services;

namespace Tourest.TourGuide.Controllers
{
    public class TourGuideController: Controller
    {
        private readonly ITourAssignmentService _tourAssignmentService;

        public TourGuideController(ITourAssignmentService tourAssignmentService)
        {
            _tourAssignmentService = tourAssignmentService;
        }


        public async Task<ActionResult> Index(int tourGuideId)
        {
            var assignedTours = await _tourAssignmentService.GetTourAssignmentsAsync(3);

            
            ViewData["AssignedTours"] = assignedTours;

            return View();
        }
        public async Task<IActionResult> TourGuideScheduleWork(int tourGuideId, int tourGroupId)
        {
         
            var schedule = await _tourAssignmentService.GetTourAssignmentsAsync(3);

            return View("TourGuideScheduleWork", schedule);
        }

    }
}
