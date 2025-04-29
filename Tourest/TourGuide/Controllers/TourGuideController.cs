using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tourest.Data;

using Tourest.Data.Entities;
using Tourest.TourGuide.Services;
using Tourest.Util;
using static Tourest.Controllers.SendEmailController;
using Tourest.Util;
using Tourest.Services;

[Route("TourGuide")]
public class TourGuideController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ITourAssignmentService _assignmentService;

    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public TourGuideController(ApplicationDbContext context, ITourAssignmentService assignmentService, INotificationService notificationService,
        IEmailService emailService)
    {
        _context = context;
        _assignmentService = assignmentService;
        _notificationService = notificationService;
        _emailService = emailService;

    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index(int tourGuideId)
    {
        await UpdateExpiredAssignmentsAsync();
        var assignments = await _assignmentService.GetTourAssignmentsAsync(3);
        return View(assignments);
    }
    [HttpGet("TourGuideScheduleWork")]
    public async Task<IActionResult> TourGuideScheduleWork(int tourGuideId, int tourGroupId)
    {

        var schedule = await _assignmentService.GetTourAssignmentsAsync(3);

        return View("TourGuideScheduleWork", schedule);
    }

    public TourGuide GetTourGuidebyID(int id)
    {
        var tourGuide = _context.TourGuides.FirstOrDefault(tg => tg.TourGuideUserID == id);
        return tourGuide;
    }



    // POST: TourGuide/AcceptAssignment
    [HttpPost("AcceptAssignment")]
    public async Task<IActionResult> AcceptAssignment([FromBody] AssignmentRequest request)
    {
        EmailRequest emailRequest = new EmailRequest();
        var TourGuideId = 3;
        emailRequest.htmlbody = MailUtil.AssignTourGuide(GetTourGuidebyID(TourGuideId));
        //_emailSerivce.SendEmail("trangtran.170204@gmail.com", "TOUREST: Xác nhận đặt tour thành công", emailRequest.htmlbody);
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
            var assignment = await _context.TourGuideAssignments
               .Include(a => a.TourGroup)
               .ThenInclude(tg => tg.Bookings)
               .FirstOrDefaultAsync(a => a.AssignmentID == request.AssignmentId);

            if (assignment?.TourGroup == null)
            {
                Console.WriteLine("Assignment or TourGroup not found.");
                return Json(new { success = true }); // Proceed without notifications if no TourGroup
            }

            // Get all bookings for the TourGroup
            var bookings = assignment.TourGroup.Bookings;
            if (!bookings.Any())
            {
                Console.WriteLine("No bookings found for this TourGroup.");
                return Json(new { success = true }); // Proceed without notifications if no bookings
            }

            // Iterate through bookings to notify each customer
            foreach (var booking in bookings)
            {
                // Fetch customer details
                var customer = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserID == booking.CustomerID);

                if (customer == null || string.IsNullOrEmpty(customer.Email))
                {
                    Console.WriteLine($"Customer not found or email is missing for CustomerID: {booking.CustomerID}.");
                    continue; // Skip to next booking
                }

                // Prepare notification
                //var notificationView = new NotificationViewModel
                //{
                //    SenderUserID = 3, // Assuming tour guide ID is 3; adjust as needed
                //    RecipientUserID = booking.CustomerID,
                //    Type = "AssignmentAccepted",
                //    Title = "Tour Guide Assignment Accepted",
                //    Content = $"The tour guide has accepted your tour assignment (ID: {request.AssignmentId}).",
                //    RelatedEntityID = "TourAssignment",
                //    RelatedEntityType = "TourAssignment",
                //    ActionUrl = $"/Tour/Details/{assignment.TourGroupID}" // Adjust URL as needed
                //};

                // Send SignalR notification
                //await _notificationService.SendingMessage(booking.CustomerID, notificationView);

                // Prepare and send email
                string subject = "Tour Guide Assignment Accepted";
                string htmlBody = $@"<h3>Tour Assignment Accepted</h3>
                    <p>Dear {customer.FullName},</p>
                    <p>The tour guide has accepted your tour assignment (ID: {request.AssignmentId}).</p>
                    <p><a href='https://yourwebsite.com/Tour/Details/{assignment.TourGroupID}'>View Tour Details</a></p>
                    <p>Thank you for choosing our service!</p>";

                bool emailSent = _emailService.SendEmail(customer.Email, subject, htmlBody);
                if (!emailSent)
                {
                    Console.WriteLine($"Failed to send email to {customer.Email}.");
                }
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
    public async Task<IActionResult> GetAssignmentDetails(int assignmentId)
    {
        if (assignmentId <= 0)
            return BadRequest("Invalid assignment ID.");

        try
        {
            
            int tourGuideId = 3;
            var assignments = await _assignmentService.GetTourAssignmentsAsync(tourGuideId);
            var assignment = assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                return NotFound(new { message = $"Assignment {assignmentId} not found." });

            // Trả về JSON của viewmodel
            return Json(assignment);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching assignment details for ID {assignmentId}: {ex}");
            return StatusCode(500, new { message = "An error occurred. Please try again later." });
        }
    }
    public async Task UpdateExpiredAssignmentsAsync()
    {
        var now = DateTime.UtcNow;
        var expiredAssignments = await _context.TourGuideAssignments
            .Where(a => a.Status == "Pending" && now > a.AssignmentDate.AddHours(12))
            .ToListAsync();

        foreach (var assignment in expiredAssignments)
        {
            assignment.Status = "Expired";
        }

        if (expiredAssignments.Any())
        {
            await _context.SaveChangesAsync();
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