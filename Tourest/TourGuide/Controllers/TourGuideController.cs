using Microsoft.AspNetCore.Mvc;
using Tourest.Data;
using Tourest.TourGuide.Services;

[Route("TourGuide")]
public class TourGuideController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ITourAssignmentService _assignmentService;

    public TourGuideController(ApplicationDbContext context, ITourAssignmentService assignmentService)
    {
        _context = context;
        _assignmentService = assignmentService;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index(int tourGuideId)
    {
        var assignments = await _assignmentService.GetTourAssignmentsAsync(3);
        ViewData["AssignedTours"] = assignments;
        return View();
    }
    [HttpGet("TourGuideScheduleWork")]
    public async Task<IActionResult> TourGuideScheduleWork(int tourGuideId, int tourGroupId)
    {

        var schedule = await _assignmentService.GetTourAssignmentsAsync(3);

        return View("TourGuideScheduleWork", schedule);
    }


    
  

    // POST: TourGuide/AcceptAssignment
    [HttpPost("AcceptAssignment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptAssignment([FromBody] AssignmentRequest request)
    {
        try
        {
            Console.WriteLine($"Accepting assignment ID: {request?.AssignmentId}");

            if (request?.AssignmentId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid assignment ID"
                });
            }

            var result = await _assignmentService.AcceptAssignmentAsync(request.AssignmentId);

            if (!result.success)
            {
                return Json(new
                {
                    success = false,
                    message = result.message
                });
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accepting assignment: {ex}");
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    [HttpPost("RejectAssignment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectAssignment([FromBody] RejectAssignmentRequest request)
    {
        try
        {
            Console.WriteLine($"Rejecting assignment ID: {request?.AssignmentId}");

            if (request?.AssignmentId <= 0 || string.IsNullOrEmpty(request?.Reason))
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid request data"
                });
            }

            var result = await _assignmentService.RejectAssignmentAsync(request.AssignmentId, request.Reason);

            if (!result.success)
            {
                return Json(new
                {
                    success = false,
                    message = result.message
                });
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rejecting assignment: {ex}");
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

   
}

// Model classes
public class AssignmentRequest
{
    public int AssignmentId { get; set; }
}

public class RejectAssignmentRequest
{
    public int AssignmentId { get; set; }
    public string Reason { get; set; }
}