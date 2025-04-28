using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels.Admin;

namespace Tourest.Controllers
{
    public class AdminTourGuidesController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminTourGuidesController> _logger;

        public AdminTourGuidesController(IUserService userService, ILogger<AdminTourGuidesController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: AdminTourGuides
        public async Task<IActionResult> Index(string searchTerm = "", int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Admin accessing Tour Guide list.");
            ViewData["CurrentFilter"] = searchTerm;
            var paginatedList = await _userService.GetTourGuidesForAdminAsync(pageIndex, pageSize, searchTerm);
            return View(paginatedList); // View uses PaginatedList<AdminTourGuideViewModel>
        }

        // GET: AdminTourGuides/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Admin viewing details for Tour Guide ID: {UserId}", id);
            var viewModel = await _userService.GetTourGuideDetailsForAdminAsync(id);
            if (viewModel == null)
            {
                _logger.LogWarning("Tour Guide ID: {UserId} not found.", id);
                return NotFound();
            }
            return View(viewModel); // View uses AdminTourGuideDetailsViewModel
        }

        // GET: AdminTourGuides/Create
        public IActionResult Create()
        {
            return View(new AdminCreateTourGuideViewModel()); // Pass empty view model
        }

        // POST: AdminTourGuides/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateTourGuideViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _userService.CreateTourGuideByAdminAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã tạo Hướng dẫn viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model);
        }

        // GET: AdminTourGuides/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _userService.GetTourGuideForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model); // View uses EditTourGuideViewModel
        }

        // POST: AdminTourGuides/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTourGuideViewModel model)
        {
            if (id != model.UserId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _userService.UpdateTourGuideByAdminAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã cập nhật thông tin HDV thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model);
        }

        // POST: AdminTourGuides/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var success = await _userService.ToggleUserActiveStatusAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Không tìm thấy HDV hoặc có lỗi xảy ra.";
            }
            else
            {
                TempData["SuccessMessage"] = "Đã thay đổi trạng thái hoạt động thành công!";
            }
            return RedirectToAction(nameof(Index)); // Consider redirecting with filters
        }

        // Delete action is omitted, recommend using ToggleActive
    }
}
