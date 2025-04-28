using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Services;
using Tourest.ViewModels.TourGuideRating;
using Tourest.ViewModels.TourRating;
using Tourest.ViewModels.UserView;
namespace Tourest.Controllers
{
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly ITourService _tourService; // Inject ITourService để lấy TourDetailsViewModel
        private readonly IBookingRepository _bookingRepository; // Cần để lấy thông tin từ BookingID
        private readonly IUserRepository _userRepository;

        public RatingController(
            IRatingService ratingService, 
            ITourService tourService, 
            IBookingRepository bookingRepository, // Inject
            IUserRepository userRepository)
        {
            _ratingService = ratingService;
            _tourService = tourService;
            _bookingRepository = bookingRepository; // Lưu
            _userRepository = userRepository;// Lưu lại TourService
        }

        [Authorize(Roles = "Customer")]
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

        [Authorize(Roles = "Customer")]
        // POST: Rating/CreateTourRating
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Sử dụng Bind với Prefix để chỉ lấy dữ liệu của form RatingForm
        public async Task<IActionResult> CreateTourRating([Bind(Prefix = "RatingForm")] CreateTourRatingViewModel ratingModel)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int customerId))
            {
                return Challenge();
            }
            //var customerId = 4;

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

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> CreateTourGuideRating(int tourGroupId, int tourGuideId, int? bookingId)
        {
            tourGuideId = 3;
            if (tourGroupId <= 0 || tourGuideId <= 0) return BadRequest("Invalid IDs.");

            // Lấy thông tin Tour Guide để hiển thị
            var tourGuideUser = await _userRepository.GetGuideByIdAsync(tourGuideId); // Lấy User là guide
            if (tourGuideUser == null) return NotFound("Tour guide not found.");

            // Lấy thông tin Tour/Booking liên quan để hiển thị (tùy chọn)
            string tourName = "Tour liên quan"; // Giá trị mặc định
            DateTime? departureDate = null;
            if (bookingId.HasValue)
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId.Value); // Lấy booking kèm tour
                if (booking?.Tour != null)
                {
                    tourName = booking.Tour.Name;
                    departureDate = booking.DepartureDate;
                }
            }
            else
            {
                // Nếu không có bookingId, có thể thử lấy TourGroup từ tourGroupId để lấy tên Tour
                // var tourGroup = await _tourGroupRepository.GetByIdAsync(tourGroupId); // Cần Repo này
                // if (tourGroup?.Tour != null) tourName = tourGroup.Tour.Name;
            }


            var pageViewModel = new RateTourGuidePageViewModel
            {
                // Map thông tin cơ bản của Guide sang UserViewModel
                TourGuideInfo = new UserViewModel
                {
                    CustomerId = tourGuideUser.UserID, // Dùng CustomerId cho nhất quán hoặc đổi tên
                    FullName = tourGuideUser.FullName,
                    ProfilePictureUrl = tourGuideUser.ProfilePictureUrl
                    // Thêm các trường khác nếu UserViewModel có
                },
                RatingForm = new CreateTourGuideRatingViewModel
                {
                    TourGuideId = tourGuideId,
                    TourGroupId = tourGroupId,
                    BookingId = bookingId,
                    TourGuideName = tourGuideUser.FullName, // Tên guide cho form
                    TourName = tourName,                   // Tên tour cho form
                    DepartureDate = departureDate          // Ngày đi cho form
                }
            };

            return View(pageViewModel); // Trả về View Views/Rating/CreateTourGuideRating.cshtml
        }

        [Authorize(Roles = "Customer")]
        // POST: Rating/CreateTourGuideRating
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTourGuideRating([Bind(Prefix = "RatingForm")] CreateTourGuideRatingViewModel ratingModel)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int customerId))
            {
                return Challenge();
            }
            //var customerId = 4;
            if (ModelState.IsValid)
            {
                var (success, message) = await _ratingService.AddTourGuideRatingAsync(ratingModel, customerId);
                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    // Chuyển về lịch sử booking
                    return RedirectToAction("Index", "BookingHistory");
                }
                else
                {
                    ModelState.AddModelError("", message);
                    TempData["ErrorMessage"] = message;
                }
            }

            // Nếu lỗi, tải lại thông tin và hiển thị lại form
            //var tourGuideUser = await _userRepository.GetGuideByIdAsync(ratingModel.TourGuideId);
            var tourGuideUser = await _userRepository.GetGuideByIdAsync(3);
            if (tourGuideUser == null) return NotFound("Tour guide not found while handling error.");

            string tourName = "Tour liên quan";
            DateTime? departureDate = null;
            if (ratingModel.BookingId.HasValue)
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(ratingModel.BookingId.Value);
                if (booking?.Tour != null) { tourName = booking.Tour.Name; departureDate = booking.DepartureDate; }
            }

            // Tạo lại Page ViewModel với dữ liệu cũ và lỗi
            var pageViewModel = new RateTourGuidePageViewModel
            {
                TourGuideInfo = new UserViewModel { /* ... Map lại thông tin guide ... */ FullName = tourGuideUser.FullName, ProfilePictureUrl = tourGuideUser.ProfilePictureUrl },
                RatingForm = ratingModel // Giữ lại model có lỗi validation
            };
            pageViewModel.RatingForm.TourName = tourName; // Gán lại tên tour
            pageViewModel.RatingForm.DepartureDate = departureDate; // Gán lại ngày đi

            return View(pageViewModel); // Trả về View với lỗi
        }
    }
}
