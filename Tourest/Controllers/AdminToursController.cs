using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels.Admin.AdminTour;

namespace Tourest.Controllers
{
    public class AdminToursController : Controller
    {
        private readonly ITourService _tourService;
        private readonly ICategoryService _categoryService; // Inject thêm để lấy categories cho form
        private readonly ILogger<AdminToursController> _logger;

        public AdminToursController(ITourService tourService, ICategoryService categoryService, ILogger<AdminToursController> logger)
        {
            _tourService = tourService;
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: AdminTours
        public async Task<IActionResult> Index(string? searchTerm = null, string? statusFilter = null, int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Admin accessing Tours list. Search: {Search}, Status: {Status}, Page: {Page}", searchTerm, statusFilter, pageIndex);
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentStatus"] = statusFilter;
            // Có thể thêm danh sách Status vào ViewData để tạo dropdown filter
            ViewData["Statuses"] = new List<string> { "Active", "Inactive", "Draft" };

            var paginatedList = await _tourService.GetToursForAdminAsync(pageIndex, pageSize, searchTerm, statusFilter);
            return View(paginatedList);
        }

        // GET: AdminTours/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Admin viewing details for TourID: {TourId}", id);
            var viewModel = await _tourService.GetTourDetailsForAdminAsync(id);
            if (viewModel == null)
            {
                _logger.LogWarning("TourID: {TourId} not found.", id);
                return NotFound();
            }
            return View(viewModel);
        }

        // GET: AdminTours/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Admin accessing Create Tour page.");
            var viewModel = await _tourService.GetTourForCreateAsync();
            if (viewModel == null)
            {
                // Xử lý lỗi nếu không lấy được categories
                TempData["ErrorMessage"] = "Không thể tải danh mục. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // POST: AdminTours/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditTourViewModel model, List<IFormFile>? imageFiles) // Nhận thêm imageFiles
        {
            _logger.LogInformation("Admin submitting Create Tour form for: {TourName}", model.Name);
            // Bỏ qua validation cho các thuộc tính chỉ dùng cho Edit
            ModelState.Remove("TourID");
            ModelState.Remove("ExistingImagePublicIds");
            ModelState.Remove("ImagesToDeletePublicIds");


            if (ModelState.IsValid)
            {
                var (success, errorMessage, createdTourId) = await _tourService.CreateTourAsync(model, imageFiles);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã tạo Tour #{createdTourId} thành công!" + (string.IsNullOrEmpty(errorMessage) ? "" : $" ({errorMessage})");
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            // Nếu ModelState không hợp lệ hoặc tạo thất bại, load lại AvailableCategories
            _logger.LogWarning("Create Tour failed for {TourName}. ModelState Valid: {IsValid}", model.Name, ModelState.IsValid);
            var allCategories = await _categoryService.GetAllCategoriesForDisplayAsync()
                ; // Cần ICategoryService
            model.AvailableCategories = allCategories.Select(c => new CategorySelectionViewModel
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                IsSelected = model.SelectedCategoryIds?.Contains(c.CategoryId) ?? false // Giữ lại lựa chọn nếu có lỗi
            }).ToList();
            // ItineraryDays và Images sẽ mất nếu không dùng JS lưu trữ tạm, cần load lại nếu cần
            return View(model);
        }

        // GET: AdminTours/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Admin accessing Edit Tour page for TourID: {TourId}", id);
            var viewModel = await _tourService.GetTourForEditAsync(id);
            if (viewModel == null)
            {
                _logger.LogWarning("TourID: {TourId} not found for edit.", id);
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: AdminTours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEditTourViewModel model, List<IFormFile>? newImageFiles, List<string>? imagesToDeletePublicIds)
        {
            _logger.LogInformation("Admin submitting Edit Tour form for TourID: {TourId}", id);
            if (id != model.TourID)
            {
                _logger.LogWarning("BadRequest: Route ID {RouteId} does not match Model ID {ModelId}", id, model.TourID);
                return BadRequest();
            }

            // Bỏ qua validation cho NewImageFiles vì nó không bắt buộc và xử lý riêng
            ModelState.Remove("NewImageFiles");


            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _tourService.UpdateTourAsync(model, newImageFiles, imagesToDeletePublicIds);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã cập nhật Tour thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            // Nếu ModelState lỗi hoặc cập nhật thất bại, load lại dữ liệu cần thiết cho View Edit
            _logger.LogWarning("Update Tour failed for {TourId}. ModelState Valid: {IsValid}", model.TourID, ModelState.IsValid);
            var tourDataForEdit = await _tourService.GetTourForEditAsync(id); // Lấy lại dữ liệu để populate View
            if (tourDataForEdit != null)
            {
                model.AvailableCategories = tourDataForEdit.AvailableCategories;
                model.ExistingImagePublicIds = tourDataForEdit.ExistingImagePublicIds;
                model.ItineraryDays = tourDataForEdit.ItineraryDays; // Load lại cả Itinerary
            }
            else
            {
                // Xử lý trường hợp tour bị xóa trong khi đang edit?
                ModelState.AddModelError(string.Empty, "Không thể tải lại dữ liệu tour để sửa.");
            }
            return View(model);
        }

        // GET: AdminTours/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Admin accessing Delete confirmation for TourID: {TourId}", id);
            var viewModel = await _tourService.GetTourForDeleteConfirmationAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel); 
        }

        // POST: AdminTours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Admin confirming deletion for TourID: {TourId}", id);
            var (success, errorMessage) = await _tourService.DeleteTourAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa Tour thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
