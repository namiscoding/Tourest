using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.ViewModels.Category;

namespace Tourest.Controllers
{
    public class AdminCategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<AdminCategoriesController> _logger;

        public AdminCategoriesController(ICategoryService categoryService, ILogger<AdminCategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: AdminCategories
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Admin accessing Category list.");
            var categories = await _categoryService.GetAllCategoriesForDisplayAsync();
            return View(categories); // Model là IEnumerable<CategoryViewModel>
        }

        // GET: AdminCategories/Details/5 (Optional)
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category); // Model là CategoryViewModel
        }

        // GET: AdminCategories/Create
        public IActionResult Create()
        {
            return View(new CategoryInputViewModel()); // Truyền ViewModel rỗng
        }

        // POST: AdminCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryInputViewModel model)
        {
            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _categoryService.CreateCategoryAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã tạo danh mục thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model);
        }

        // GET: AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _categoryService.GetCategoryForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model); // Model là CategoryInputViewModel đã có dữ liệu
        }

        // POST: AdminCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryInputViewModel model)
        {
            if (id != model.CategoryID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _categoryService.UpdateCategoryAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đã cập nhật danh mục thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model);
        }

        // GET: AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            // Có thể kiểm tra IsInUse ở đây và đưa cảnh báo vào ViewData/ViewBag
            // ViewBag.IsInUse = await _categoryRepository.IsInUseAsync(id); // Cần inject Repo hoặc thêm method vào Service
            return View(category); // Model là CategoryViewModel
        }

        // POST: AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, errorMessage) = await _categoryService.DeleteCategoryAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa danh mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage; // Hiển thị lỗi (ví dụ: đang được sử dụng)
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
