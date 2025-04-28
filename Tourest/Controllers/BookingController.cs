using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.ViewModels.Booking;

namespace Tourest.Controllers
{
    // [Authorize] 
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly ITourService _tourService; 

        public BookingController(IBookingService bookingService, ITourService tourService)
        {
            _bookingService = bookingService;
            _tourService = tourService;
        }
        [Authorize(Roles = "Customer")]
        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingViewModel model)
        {
            //var customerId = 4;

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int customerId))
            {
                return Challenge();
            }


            if (ModelState.IsValid)
            {
                var (success, message, bookingId) = await _bookingService.CreateBookingAsync(model, customerId);

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    // Chuyển hướng đến trang thanh toán (VÍ DỤ)
                    return RedirectToAction("Index", "BookingHistory");
                }
                else
                {
                    
                    ModelState.AddModelError(string.Empty, message); 
                    TempData["ErrorMessage"] = message; 
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Thông tin đặt tour không hợp lệ. Vui lòng kiểm tra lại.";
            }


            // Nếu ModelState không hợp lệ hoặc có lỗi từ Service, tải lại trang Details
            var tourDetailsViewModel = await _tourService.GetTourDetailsAsync(model.TourId);
            if (tourDetailsViewModel == null) return NotFound("Tour không tồn tại.");

            // Có thể cần truyền lại dữ liệu người dùng đã nhập (model) để điền lại form
            // Ví dụ: Dùng ViewData hoặc thêm thuộc tính cho CreateBookingViewModel vào TourDetailsViewModel
            ViewData["BookingFormModel"] = model; // Truyền model lỗi qua ViewData


            return View("~/Views/Tours/Details.cshtml", tourDetailsViewModel); // Trả về view Details
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Tourest.Services;
//using Tourest.ViewModels.Booking; // Namespace của CreateBookingViewModel
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System;

//namespace Tourest.Controllers
//{
//    [Authorize] // Yêu cầu đăng nhập để đặt tour
//    public class BookingController : Controller
//    {
//        private readonly IBookingService _bookingService;
//        private readonly ITourService _tourService; // Có thể cần để lấy lại thông tin hiển thị form nếu lỗi

//        public BookingController(IBookingService bookingService, ITourService tourService)
//        {
//            _bookingService = bookingService;
//            _tourService = tourService;
//        }

//        // GET: Booking/Create - Có thể dùng để hiển thị form đặt riêng nếu cần
//        // Hoặc Action này được gọi từ nút "Đặt Ngay" trên trang Tour Details

//        // POST: Booking/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(CreateBookingViewModel model) // Nhận ViewModel mới
//        {
//            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (!int.TryParse(userIdString, out int customerId))
//            {
//                // Chuyển hướng đăng nhập hoặc báo lỗi rõ ràng
//                return Challenge(); // Hoặc return Unauthorized("Vui lòng đăng nhập.");
//            }

//            // Kiểm tra dữ liệu đầu vào từ form có hợp lệ không
//            if (ModelState.IsValid)
//            {
//                // Gọi Service để xử lý logic tạo Booking
//                var (success, message, bookingId) = await _bookingService.CreateBookingAsync(model, customerId);

//                if (success)
//                {
//                    // Thành công, chuyển hướng đến trang thanh toán hoặc trang xác nhận
//                    TempData["SuccessMessage"] = message;
//                    // Ví dụ: Chuyển đến trang thanh toán với bookingId
//                    return RedirectToAction("Process", "Payment", new { bookingId = bookingId });
//                    // Hoặc chuyển về trang lịch sử booking
//                    // return RedirectToAction("Index", "BookingHistory");
//                }
//                else
//                {
//                    // Thất bại (ví dụ: hết chỗ, lỗi...), thêm lỗi vào ModelState
//                    ModelState.AddModelError("", message);
//                }
//            }

//            // Nếu ModelState không hợp lệ hoặc có lỗi từ Service
//            // Cần tải lại dữ liệu cho trang Tour Details và hiển thị lại form với lỗi
//            TempData["ErrorMessage"] = "Đặt tour không thành công. Vui lòng kiểm tra lại thông tin.";

//            // Lấy lại dữ liệu cho trang Tour Details để hiển thị lại form
//            // Do model không chứa đủ thông tin của TourDetailsViewModel, nên cần lấy lại
//            var tourDetailsViewModel = await _tourService.GetTourDetailsAsync(model.TourId);
//            if (tourDetailsViewModel == null) return NotFound("Tour không tồn tại.");

//            // Có thể gán lại các giá trị người dùng đã nhập vào một đối tượng CreateBookingViewModel
//            // trong tourDetailsViewModel để hiển thị lại, hoặc dùng ViewData/ViewBag

//            // Truyền lại model lỗi vào ViewData để View có thể lấy lại
//            ViewData["BookingModel"] = model;


//            // Trả về View của trang Chi tiết Tour (KHÔNG phải view Create của Booking)
//            return View("~/Views/Tours/Details.cshtml", tourDetailsViewModel);
//        }

//        // Action Details cho Booking (nếu cần)
//        // public async Task<IActionResult> Details(int id) { ... }
//    }
//}