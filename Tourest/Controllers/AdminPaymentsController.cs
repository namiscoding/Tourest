using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Tourest.Services;

namespace Tourest.Controllers
{
    public class AdminPaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<AdminPaymentsController> _logger;

        public AdminPaymentsController(IPaymentService paymentService, ILogger<AdminPaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // GET: AdminPayments
        public async Task<IActionResult> Index(
            string? methodFilter = null,
            string? startDate = null,
            string? endDate = null,
            string? searchTerm = null,
            string? sortBy = "date", // Mặc định sort theo date
            bool ascending = false, // Mặc định giảm dần (mới nhất trước)
            int pageIndex = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Admin accessing Payment list.");

            // Xử lý parse ngày tháng
            DateTime? startDt = null;
            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStart))
            {
                startDt = parsedStart;
            }
            DateTime? endDt = null;
            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedEnd))
            {
                endDt = parsedEnd.AddDays(1).AddTicks(-1); // Lấy hết ngày cuối cùng
            }


            ViewData["CurrentMethod"] = methodFilter;
            ViewData["CurrentStartDate"] = startDate;
            ViewData["CurrentEndDate"] = endDate;
            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentSort"] = sortBy;
            ViewData["Ascending"] = ascending;

            // Lấy danh sách Payment Methods và Statuses để tạo dropdown filter (có thể lấy từ DB hoặc hardcode)
            ViewData["PaymentMethods"] = new List<string> { "MoMo", "CreditCard", "BankTransfer", "VNPay" }; // Ví dụ
            ViewData["Statuses"] = new List<string> { "Completed", "Pending", "Failed" }; // Ví dụ


            var paginatedList = await _paymentService.GetPaymentsForAdminAsync(
                methodFilter, startDt, endDt, searchTerm, sortBy, ascending, pageIndex, pageSize);

            return View(paginatedList); // Model là PaginatedList<AdminPaymentListViewModel>
        }

        // GET: AdminPayments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Admin viewing details for PaymentID: {PaymentId}", id);
            var viewModel = await _paymentService.GetPaymentDetailsForAdminAsync(id);
            if (viewModel == null)
            {
                _logger.LogWarning("PaymentID: {PaymentId} not found.", id);
                return NotFound();
            }
            return View(viewModel); // Model là AdminPaymentDetailsViewModel
        }
    }
}
