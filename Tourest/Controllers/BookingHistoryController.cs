using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourest.Data.Entities;
using Tourest.Services;

namespace Tourest.Controllers
{
    public class BookingHistoryController : Controller
    {
        private readonly IBookingService _bookingService;
        // Inject thêm IUserService hoặc UserManager nếu cần lấy thông tin user khác ngoài ID

        public BookingHistoryController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [Authorize(Roles = "Customer")]
        // GET: /BookingHistory hoặc /BookingHistory?destinationFilter=SomePlace
        public async Task<IActionResult> Index(string? destinationFilter)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int customerId))
            {
                return Challenge();
            }
            
            //if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int customerId))
            //{
            //    // Xử lý trường hợp không lấy được UserID (chưa đăng nhập hoặc cấu hình claim sai)
            //    // Có thể chuyển hướng đến trang đăng nhập hoặc trả về lỗi
            //    return Challenge(); // Ví dụ: yêu cầu đăng nhập lại
            //                        // Hoặc return Unauthorized();
            //                        // Hoặc return BadRequest("User ID not found.");
            //}
            // --- Kết thúc lấy User ID ---

            var viewModel = await _bookingService.GetBookingHistoryAsync(customerId, destinationFilter);

            return View(viewModel); // Trả về View Index với ViewModel
        }
    }
}
