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
        return View(assignments);
    }
    [HttpGet("TourGuideScheduleWork")]
    public async Task<IActionResult> TourGuideScheduleWork(int tourGuideId, int tourGroupId)
    {

        var schedule = await _assignmentService.GetTourAssignmentsAsync(3);

        return View("TourGuideScheduleWork", schedule);
    }





    // POST: TourGuide/AcceptAssignment
    [HttpPost("AcceptAssignment")]
    public async Task<IActionResult> AcceptAssignment([FromBody] AssignmentRequest request)
    {
        try
        {
            Console.WriteLine($"Accepting assignment ID: {request?.AssignmentId}");

            if (request?.AssignmentId <= 0)
            {
                return Json(new { success = false, message = "Invalid assignment ID" });
            }

            var result = await _assignmentService.AcceptAssignmentAsync(request.AssignmentId);

            if (!result.success)
            {
                return Json(new { success = false, message = result.message });
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accepting assignment: {ex}");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // POST: TourGuide/RejectAssignment
    [HttpPost("RejectAssignment")]
    public async Task<IActionResult> RejectAssignment([FromBody] RejectAssignmentRequest request)
    {
        try
        {
            Console.WriteLine($"Rejecting assignment ID: {request?.AssignmentId}");

            if (request?.AssignmentId <= 0 || string.IsNullOrEmpty(request?.Reason))
            {
                return Json(new { success = false, message = "Invalid request data" });
            }

            var result = await _assignmentService.RejectAssignmentAsync(request.AssignmentId, request.Reason);

            if (!result.success)
            {
                return Json(new { success = false, message = result.message });
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rejecting assignment: {ex}");
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpGet("GetTourFeedback")]
    public IActionResult GetTourFeedback(int tourId)
    {
        Console.WriteLine($"Fetching feedback for AssignmentID: {tourId}");
        var feedback = (from tga in _context.TourGuideAssignments
                        join tr in _context.TourGuideRatings on tga.TourGroupID equals tr.TourGroupID
                        join r in _context.Ratings on tr.RatingID equals r.RatingID
                        join u in _context.Users on r.CustomerID equals u.UserID
                        where tga.AssignmentID == tourId && tga.TourGuideID == 3
                        select new
                        {
                            ratingId = r.RatingID,
                            customerName = u.FullName,
                            //customerAvatar = u.AvatarUrl ?? "https://via.placeholder.com/40",
                            ratingValue = r.RatingValue,
                            comment = r.Comment,
                            ratingDate = r.RatingDate,
                            guideResponse = (string)null,
                            responseDate = (DateTime?)null
                        }).ToList();

        
        return Json(feedback);
    }
    [HttpGet("GetAssignmentDetails")]
    public async Task<IActionResult> GetAssignmentDetails(int assignmentId, int tourGuideId)
    {
        if (assignmentId <= 0 || tourGuideId <= 0)
        {
            return BadRequest("Invalid assignment ID or tour guide ID.");
        }

        try
        {
            var assignments = await _assignmentService.GetTourAssignmentsAsync(tourGuideId);
            var assignment = assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);

            if (assignment == null)
            {
                return NotFound($"Assignment with ID {assignmentId} not found for tour guide ID {tourGuideId}.");
            }

            return PartialView("_AssignmentDetails", assignment);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching assignment details for ID {assignmentId}: {ex.Message}");
            return StatusCode(500, "An error occurred while fetching assignment details. Please try again later.");
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