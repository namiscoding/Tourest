using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Thêm using này
using System;
using Tourest.Data;
using Tourest.Data.Entities.Momo;
using Tourest.Services.Momo; // Thêm using này
// ... các using khác ...

namespace Tourest.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CheckoutController> _logger; // Thêm logger

        public CheckoutController(IMomoService momoService, ApplicationDbContext dbContext, ILogger<CheckoutController> logger) // Inject logger
        {
            _momoService = momoService ?? throw new ArgumentNullException(nameof(momoService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger; // Gán logger
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            _logger.LogInformation("===== Entering PaymentCallBack =====");
            _logger.LogInformation("QueryString: {Query}", HttpContext.Request.QueryString.Value ?? "empty"); // Log toàn bộ query string

            var requestQuery = HttpContext.Request.Query;

            // *** TODO: Thêm xác thực chữ ký MoMo ở đây ***

            try
            {
                _logger.LogInformation("Attempting to execute PaymentExecuteAsync");
                var response = _momoService.PaymentExecuteAsync(requestQuery);
                _logger.LogInformation("PaymentExecuteAsync completed. OrderId: {OrderId}, Amount: {Amount}, ResultCode: {ResultCode}",
                                       requestQuery["orderId"], requestQuery["amount"], requestQuery["resultCode"]);

                if (requestQuery["resultCode"] == "0") // Thành công
                {
                    _logger.LogInformation("Payment successful (resultCode == 0). Processing...");

                    _logger.LogInformation("Creating MomoInfoModel object.");
                    var newMomoInfo = new MomoInfoModel
                    {
                        OrderId = response.OrderId,
                        // FullName = ..., // Lấy FullName nếu cần
                        Amount = decimal.Parse(response.Amount), // Có thể cần try-catch riêng cho parse này
                        OrderInfo = response.OrderInfo,
                        DatePaid = DateTime.Now
                    };
                    _logger.LogInformation("MomoInfoModel created. OrderId: {OrderId}", newMomoInfo.OrderId);

                    _logger.LogInformation("Attempting to add MomoInfoModel to DbContext.");
                    _dbContext.Add(newMomoInfo);
                    _logger.LogInformation("Added to DbContext.");

                    _logger.LogInformation("Attempting to SaveChangesAsync.");
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("SaveChangesAsync successful.");

                    // ... (Xử lý cập nhật Booking nếu có + thêm logging) ...

                    TempData["success"] = "Thanh toán MoMo thành công!";
                    _logger.LogInformation("Returning View with success message.");
                    return View(response);
                }
                else // Thất bại
                {
                    _logger.LogWarning("Payment failed (resultCode != 0). ResultCode: {ResultCode}, Message: {Message}",
                                       requestQuery["resultCode"], requestQuery["message"]);
                    TempData["error"] = $"Giao dịch Momo không thành công. Lỗi: {requestQuery["message"]}";
                    return RedirectToAction("Index", "BookingHistory");
                }
            }
            catch (Exception ex)
            {
                // LỖI XẢY RA Ở ĐÂY! Log chi tiết lỗi
                _logger.LogError(ex, "!!!!! EXCEPTION caught in PaymentCallBack !!!!!");
                // Log thêm các thông tin quan trọng có thể gây lỗi
                _logger.LogError("Request Query Again: {Query}", HttpContext.Request.QueryString.Value ?? "empty");

                // Có thể throw lại lỗi để middleware xử lý, hoặc trả về trang lỗi chung
                TempData["error"] = "Đã có lỗi nghiêm trọng xảy ra trong quá trình xử lý thanh toán.";
                // QUAN TRỌNG: Không trả về View(response) ở đây vì có thể response chưa được tạo hoặc lỗi.
                // Nên redirect về trang lỗi hoặc trang lịch sử.
                return RedirectToAction("Index", "BookingHistory"); // Hoặc trang Error
                // Hoặc throw; // Để middleware xử lý exception (nếu có cấu hình)
            }
        }
    }
}