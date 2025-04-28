using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels.Admin;

namespace Tourest.Controllers
{
    public class AdminTourManagersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminTourManagersController> _logger;

        public AdminTourManagersController(IUserService userService, ILogger<AdminTourManagersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /AdminTourManagers
        public async Task<IActionResult> Index(string searchTerm = "", int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Admin accessing Tour Manager list.");
            ViewData["CurrentFilter"] = searchTerm;
            var paginatedList = await _userService.GetTourManagersForAdminAsync(pageIndex, pageSize, searchTerm);
            return View(paginatedList); // View này dùng model PaginatedList<AdminTourManagerViewModel>
        }

        // GET: /AdminTourManagers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Admin viewing details for Tour Manager ID: {UserId}", id);
            var viewModel = await _userService.GetTourManagerDetailsForAdminAsync(id);
            if (viewModel == null)
            {
                _logger.LogWarning("Tour Manager ID: {UserId} not found.", id);
                return NotFound();
            }
            return View(viewModel); // View này dùng model AdminTourManagerDetailsViewModel
        }

        // GET: /AdminTourManagers/Create
        public IActionResult Create()
        {
            return View(); // View này dùng model AdminCreateTourManagerViewModel
        }

        // POST: /AdminTourManagers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateTourManagerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _userService.CreateTourManagerByAdminAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã tạo Quản lý Tour thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model); // Trả về form nếu có lỗi
        }

        // GET: /AdminTourManagers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _userService.GetTourManagerForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model); // View này dùng model EditTourManagerViewModel
        }

        // POST: /AdminTourManagers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTourManagerViewModel model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _userService.UpdateTourManagerByAdminAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã cập nhật thông tin thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model);
        }

        // POST: /AdminTourManagers/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var success = await _userService.ToggleUserActiveStatusAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng hoặc có lỗi xảy ra.";
            }
            else
            {
                TempData["SuccessMessage"] = "Đã thay đổi trạng thái hoạt động thành công!";
            }
            // Cần lấy lại các tham số tìm kiếm/phân trang nếu muốn quay về đúng trang Index trước đó
            return RedirectToAction(nameof(Index));
        }

        // Không nên có action Delete trực tiếp, dùng ToggleActive
    }
}
