using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.ViewModels.TourRating;

namespace Tourest.Controllers
{
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly ITourService _tourService; // Inject ITourService để lấy TourDetailsViewModel

        public RatingController(IRatingService ratingService, ITourService tourService)
        {
            _ratingService = ratingService;
            _tourService = tourService; // Lưu lại TourService
        }

        // GET: Rating/CreateTourRating?tourId=1&bookingId=10
        [HttpGet]
        public async Task<IActionResult> CreateTourRating(int tourId, int? bookingId)
        {
            if (tourId <= 0) return BadRequest("Invalid Tour ID.");

            // Lấy thông tin chi tiết tour bằng TourService
            var tourDetails = await _tourService.GetTourDetailsAsync(tourId);
            if (tourDetails == null) return NotFound("Tour not found.");

            // Tạo ViewModel cho trang đánh giá
            var pageViewModel = new RateTourPageViewModel
            {
                TourDetails = tourDetails, // Gán thông tin tour chi tiết
                RatingForm = new CreateTourRatingViewModel // Tạo model rỗng cho form
                {
                    TourId = tourId,
                    BookingId = bookingId,
                    TourName = tourDetails.Name // Lấy tên tour cho form
                }
            };

            return View(pageViewModel); // Trả về View với ViewModel trang
        }

        // POST: Rating/CreateTourRating
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Sử dụng Bind với Prefix để chỉ lấy dữ liệu của form RatingForm
        public async Task<IActionResult> CreateTourRating([Bind(Prefix = "RatingForm")] CreateTourRatingViewModel ratingModel)
        {
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (!int.TryParse(userIdString, out int customerId))
            //{
            //    return Challenge();
            //}
            var customerId = 4;

            // Kiểm tra ModelState chỉ cho phần RatingForm
            if (ModelState.IsValid)
            {
                // Gọi service để lưu đánh giá
                var (success, message) = await _ratingService.AddTourRatingAsync(ratingModel, customerId);

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    // Chuyển hướng về trang lịch sử booking hoặc trang chi tiết tour
                    return RedirectToAction("Index", "BookingHistory");
                    // Hoặc return RedirectToAction("Details", "Tours", new { id = ratingModel.TourId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, message);
                    TempData["ErrorMessage"] = message;
                }
            }

            // Nếu ModelState không hợp lệ hoặc có lỗi từ service:
            // Cần tải lại TourDetails và tạo lại RateTourPageViewModel đầy đủ
            var tourDetails = await _tourService.GetTourDetailsAsync(ratingModel.TourId);
            if (tourDetails == null) return NotFound("Tour not found while handling error."); // Xử lý tour không còn tồn tại

            var pageViewModel = new RateTourPageViewModel
            {
                TourDetails = tourDetails,
                RatingForm = ratingModel // Giữ lại dữ liệu người dùng đã nhập và lỗi validation
            };

            return View(pageViewModel); // Trả về View với lỗi và dữ liệu đầy đủ
        }
    }
}
