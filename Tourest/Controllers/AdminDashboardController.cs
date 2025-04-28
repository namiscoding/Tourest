using Microsoft.AspNetCore.Mvc;
using Tourest.Services;

namespace Tourest.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IBookingProcessingService _bookingProcessor;

        // Constructor giờ cũng nên đổi tên cho nhất quán (nhưng không bắt buộc)
        public AdminDashboardController(IBookingProcessingService bookingProcessor)
        {
            _bookingProcessor = bookingProcessor;
        }

        // Action Index (Mặc định cho /AdminDashboard)
        public IActionResult Index()
        {
            // Trả về View trong thư mục Views/AdminDashboard/
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
    }
}
