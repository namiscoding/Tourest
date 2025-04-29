using Microsoft.AspNetCore.Mvc;
using Tourest.Services;

namespace Tourest.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly IBookingProcessingService _bookingProcessor;

        public AdminDashboardController(IDashboardService dashboardService, ILogger<AdminDashboardController> logger, IBookingProcessingService bookingProcessor)
        {
            _dashboardService = dashboardService;
            _logger = logger;
            _bookingProcessor = bookingProcessor;
        }

        
        public async Task<IActionResult> Index()
        {
            return View();
        }


        // Action TriggerBookingUpdate (Giữ nguyên)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TriggerBookingUpdate()
        {
            var resultTuple = await _bookingProcessor.UpdateStatusesForPastDeparturesAsync();
            int count = resultTuple.updatedCount;
            string message = resultTuple.message;

            if (count >= 0)
            {
                TempData["SuccessMessage"] = $"Manual update completed. {message}";
            }
            else
            {
                TempData["ErrorMessage"] = $"Manual update failed. {message}";
            }
            // Chuyển hướng về Action Index của Controller NÀY
            return RedirectToAction(nameof(Index));
        }

        // ... Các actions khác của Admin ...
        public async Task<IActionResult> Dashboard()
        {
            _logger.LogInformation("Admin accessing dashboard.");
            var viewModel = await _dashboardService.GetDashboardDataAsync(TimePeriod.Last30Days);
            return View(viewModel);
        }
    }
}
