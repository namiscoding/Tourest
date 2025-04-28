using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels.Admin;

namespace Tourest.Controllers
{
    public class AdminCustomersController : Controller 
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminCustomersController> _logger;

        public AdminCustomersController(IUserService userService, ILogger<AdminCustomersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /AdminCustomers
        public async Task<IActionResult> Index(string searchTerm = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchTerm;
            var paginatedCustomers = await _userService.GetCustomersForAdminAsync(pageIndex, pageSize, searchTerm);
            return View(paginatedCustomers); // Truyền PaginatedList<AdminCustomerViewModel>
        }

        // GET: /AdminCustomers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _userService.GetCustomerDetailsForAdminAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // GET: /AdminCustomers/Create
        public IActionResult Create()
        {
            return View(); // Trả về View với form trống
        }

        // POST: /AdminCustomers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateCustomerByAdminAsync(model);
                if (result.Success)
                {
                    // Thêm TempData để hiển thị thông báo thành công (optional)
                    TempData["SuccessMessage"] = "Đã tạo khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Thêm lỗi vào ModelState để hiển thị trên View
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                }
            }
            // Nếu ModelState không hợp lệ hoặc tạo thất bại, trả về View với dữ liệu đã nhập
            return View(model);
        }

        // GET: /AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _userService.GetCustomerForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /AdminCustomers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCustomerViewModel model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateCustomerByAdminAsync(model);
                if (result.Success) 
                {
                    TempData["SuccessMessage"] = "Đã cập nhật thông tin khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                }
            }

            if (string.IsNullOrEmpty(model.ProfilePictureUrl)) // Lấy lại PublicId nếu update lỗi và model bị mất
            {
                var existingData = await _userService.GetCustomerForEditAsync(id);
                model.ProfilePictureUrl = existingData?.ProfilePictureUrl;
            }
            return View(model);
        }

        // POST: /AdminCustomers/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm để bảo mật hơn
        public async Task<IActionResult> ToggleActive(int id)
        {
            var success = await _userService.ToggleUserActiveStatusAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng hoặc có lỗi xảy ra.";
                // return NotFound(); // Hoặc trả về lỗi nếu cần
            }
            else
            {
                TempData["SuccessMessage"] = "Đã thay đổi trạng thái hoạt động thành công!";
            }
            // Chuyển hướng về trang Index (có thể giữ lại trang và tìm kiếm hiện tại)
            // Cần truyền lại các tham số filter/page nếu muốn giữ nguyên trạng thái trang Index
            return RedirectToAction(nameof(Index));
        }
    }
}
