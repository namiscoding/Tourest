using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.ViewModels.SupportRequest;

namespace Tourest.Controllers
{
    //[Authorize] 
    public class SupportRequestController : Controller
    {
        private readonly ISupportRequestService _supportRequestService;
        public SupportRequestController(ISupportRequestService supportRequestService) { _supportRequestService = supportRequestService; }

        
        public async Task<IActionResult> Index() 
        {
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (!int.TryParse(userIdString, out int customerId)) return Challenge();
            var customerId =4; // Gán cứng customerId là 4 để test
            var viewModel = await _supportRequestService.GetMyRequestsViewModelAsync(customerId);
            // Đảm bảo NewRequest không null nếu View cần nó để render form
            if (viewModel.NewRequest == null)
            {
                viewModel.NewRequest = new CreateSupportRequestViewModel();
            }
            return View(viewModel); 
        }

        // POST: /SupportRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Nhận model con để validation chỉ áp dụng cho các trường của form Create
        public async Task<IActionResult> Create([Bind(Prefix = "NewRequest")] CreateSupportRequestViewModel model)
        {
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (!int.TryParse(userIdString, out int customerId)) return Challenge();
            var customerId = 4;
            // Chỉ kiểm tra ModelState của model con được bind
            if (ModelState.IsValid)
            {
                try
                {
                    await _supportRequestService.CreateSupportRequestAsync(model, customerId);
                    TempData["SuccessMessage"] = "Yêu cầu hỗ trợ của bạn đã được gửi thành công!";
                    return RedirectToAction(nameof(Index)); // Chuyển hướng về trang Index để xem danh sách mới
                }
                catch (Exception ex)
                {
                    // Log lỗi
                    ModelState.AddModelError("", "Lỗi xảy ra khi gửi yêu cầu.");
                }
            }

            // Nếu ModelState không hợp lệ hoặc có lỗi xảy ra
            // Cần lấy lại toàn bộ dữ liệu cho trang Index và hiển thị lại
            var viewModel = await _supportRequestService.GetMyRequestsViewModelAsync(customerId);
            viewModel.NewRequest = model; // Giữ lại dữ liệu người dùng đã nhập trong form
            TempData["ErrorMessage"] = "Gửi yêu cầu thất bại, vui lòng kiểm tra lại thông tin."; // Thêm thông báo lỗi
            return View("Index", viewModel); // Trả về View Index với dữ liệu và lỗi
        }

    }
}
