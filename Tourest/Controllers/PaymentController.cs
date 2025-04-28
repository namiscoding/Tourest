using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization; // Required for TryParse CultureInfo
using System.Text; // Required for StringBuilder
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.Data.Entities.Momo;
using Tourest.Data.Repositories;
using Tourest.Services;
using Tourest.Services.Momo;
using Tourest.Util;
using Tourest.ViewModels.Booking;
using static Tourest.Controllers.SendEmailController; // Or the correct namespace for MomoCallbackViewModel
// Add other necessary using statements

public class PaymentController : Controller
{
    private readonly IMomoService _momoService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PaymentController> _logger;
    private readonly IEmailService _emailSerivce;
    private readonly INotificationService _notificationService;
    private readonly ITourGroupRepository _tourGroupRepository;

    public PaymentController(IMomoService momoService, 
        ApplicationDbContext dbContext, 
        ILogger<PaymentController> logger, 
        IEmailService emailSerivce, 
        INotificationService notificationService,
        ITourGroupRepository tourGroupRepository)
    {
        _momoService = momoService ?? throw new ArgumentNullException(nameof(momoService));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSerivce = emailSerivce;
        _notificationService = notificationService;
        _tourGroupRepository = tourGroupRepository;
    }

    // --- CreatePaymentMomo (Giữ nguyên như code bạn cung cấp, đã đúng) ---
    [HttpPost]
    public async Task<IActionResult> CreatePaymentMomo(MomoFormDataModel formData)
    {
        _logger.LogInformation("Received request to create MoMo payment link for existing BookingId (as OrderId): {OrderId}, Amount from form: {Amount}", formData.OrderId, formData.Amount);

        // --- KIỂM TRA DỮ LIỆU ĐẦU VÀO TỪ FORM ---
        if (formData == null || string.IsNullOrEmpty(formData.OrderId) || formData.Amount <= 0)
        {
            _logger.LogWarning("Invalid form data received for MoMo payment creation.");
            TempData["PaymentError"] = "Dữ liệu gửi đi không hợp lệ.";
            return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
        }

        // --- PARSE BOOKING ID ---
        if (!long.TryParse(formData.OrderId, out long bookingId))
        {
            _logger.LogWarning("Could not parse OrderId '{MomoOrderId}' from form data to long.", formData.OrderId);
            TempData["PaymentError"] = "Mã đơn hàng không hợp lệ.";
            return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
        }

        try
        {
            // --- TÌM BOOKING TRONG DB (KÈM THÔNG TIN KHÁCH HÀNG) ---
            var booking = await _dbContext.Bookings
                                          .Include(b => b.Customer)
                                          .Include(b => b.Tour)
                                          .Include(b => b.Payment) // Include Payment để kiểm tra Amount chính xác hơn nếu cần
                                          .FirstOrDefaultAsync(b => b.BookingID == bookingId);

            // --- VALIDATE BOOKING ---
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found.", bookingId);
                TempData["PaymentError"] = "Không tìm thấy đơn hàng để thanh toán.";
                return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
            }

            if (booking.Customer == null)
            {
                _logger.LogError("Customer information not found for Booking {BookingId}.", bookingId);
                TempData["PaymentError"] = "Thiếu thông tin khách hàng của đơn hàng.";
                return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
            }

            // Kiểm tra trạng thái booking
            if (booking.Status != "PendingPayment")
            {
                _logger.LogWarning("Booking {BookingId} is not in 'PendingPayment' status. Current status: {Status}", bookingId, booking.Status);
                TempData["PaymentError"] = $"Đơn hàng đang ở trạng thái '{booking.Status}', không thể thanh toán.";
                if (booking.Status == "Paid")
                {
                    return RedirectToAction("Details", "BookingHistory", new { id = booking.BookingID }); // <<< THAY THẾ
                }
                return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
            }

            // Kiểm tra giá trị Amount từ form với giá trị trong DB
            if (Math.Abs(booking.TotalPrice - formData.Amount) > 0.01m)
            {
                _logger.LogWarning("Amount mismatch for Booking {BookingId}. DB TotalPrice: {DbPrice}, Form Amount: {FormAmount}",
                                   bookingId, booking.TotalPrice, formData.Amount);
                TempData["PaymentError"] = "Số tiền thanh toán không khớp với đơn hàng.";
                return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
            }

            // --- TẠO ĐỊNH DANH DUY NHẤT CHO LẦN THANH TOÁN NÀY ---
            string uniqueAttemptId = $"{booking.BookingID}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"; // Ví dụ: "21_1721986487"
            _logger.LogInformation("Generating unique MoMo attempt ID: {UniqueAttemptId} for BookingID: {BookingId}", uniqueAttemptId, booking.BookingID);

            // --- CHUẨN BỊ extraData ---
            var extraDataObject = new { bookingId = booking.BookingID }; // Chỉ cần bookingId là đủ để tìm lại
            string extraDataJson = System.Text.Json.JsonSerializer.Serialize(extraDataObject); // Hoặc dùng Newtonsoft.Json
            string extraDataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(extraDataJson));
            _logger.LogInformation("Encoded extraData: {ExtraDataBase64}", extraDataBase64);


            // --- CHUẨN BỊ DỮ LIỆU GỬI CHO MOMO SERVICE ---
            var momoRequestModel = new OrderInfoModel // Model này là tham số của _momoService.CreatePaymentMomo
            {
                // Sử dụng uniqueAttemptId cho OrderId và RequestId gửi đi
                OrderId = uniqueAttemptId,
                RequestId = uniqueAttemptId, // Thường dùng giống OrderId cho MoMo
                Amount = booking.TotalPrice,
                OrderInfo = $"Thanh toán cho Tour: {booking.Tour?.Name ?? "N/A"} (BKID: {booking.BookingID})", // Thêm BKID vào info cho dễ nhìn log MoMo
                FullName = booking.Customer.FullName,
                // Thêm extraData đã encode
                ExtraData = extraDataBase64
            };

            // --- GỌI MOMO SERVICE ---
            // Service cần được cập nhật để sử dụng OrderId, RequestId, ExtraData từ momoRequestModel
            var response = await _momoService.CreatePaymentMomo(momoRequestModel);

            // --- XỬ LÝ KẾT QUẢ TỪ MOMO ---
            if (response != null && response.ResultCode == 0 && !string.IsNullOrEmpty(response.PayUrl))
            {
                // Thành công: Có thể lưu uniqueAttemptId vào booking nếu cần theo dõi lần thử cuối
                // booking.LastMomoAttemptId = uniqueAttemptId;
                // await _dbContext.SaveChangesAsync();
                _logger.LogInformation("MoMo payment URL created successfully for BookingID {BookingId} (AttemptID: {AttemptId}). Redirecting.", booking.BookingID, uniqueAttemptId);
                return Redirect(response.PayUrl);
            }
            else
            {
                // Lỗi từ MoMo (kể cả lỗi "OrderId exists" nếu logic mới vẫn bị)
                _logger.LogError("Failed to create MoMo payment URL for BookingID {BookingId} (AttemptID: {AttemptId}). MoMo ResultCode: {ResultCode}, Message: {Message}",
                                  booking.BookingID, uniqueAttemptId, response?.ResultCode, response?.Message);
                booking.Status = "Failed_CreateLink";
                // booking.PaymentNotes = $"Failed MoMo attempt {uniqueAttemptId}: {response?.Message}";
                await _dbContext.SaveChangesAsync();
                TempData["PaymentError"] = $"Không thể tạo yêu cầu thanh toán MoMo: {response?.Message ?? "Lỗi không xác định từ MoMo"}";
                return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
            }
        }
        catch (Exception ex)
        {
            // ... (xử lý exception như cũ) ...
            _logger.LogError(ex, "Exception occurred while processing MoMo payment link creation for OrderId from form: {OrderId}", formData.OrderId);
            // ... (cập nhật status lỗi nếu có thể) ...
            TempData["PaymentError"] = "Đã xảy ra lỗi hệ thống khi tạo link thanh toán MoMo.";
            return RedirectToAction("Index", "BookingHistory"); // <<< THAY THẾ
        }
    }


    // --- PaymentCallBack (GET - Xử lý Return URL) ---
    [HttpGet]
    public async Task<IActionResult> PaymentCallBack()
    {
        try
        {
            _logger.LogInformation("PaymentCallBack (Return URL) reached. QueryString: {QueryString}", Request.QueryString);
            var collection = Request.Query;

            // --- TẠO MomoCallbackViewModel THỦ CÔNG TỪ QUERY STRING ---
            var callbackData = new MomoCallbackViewModel();
            bool parseError = false; // Biến cờ để kiểm tra lỗi parse

            // --- ĐỌC VÀ GÁN GIÁ TRỊ ---
            _logger.LogInformation("Populating callbackData for Return URL...");

            collection.TryGetValue("partnerCode", out var partnerCode); callbackData.PartnerCode = partnerCode;
            _logger.LogInformation(" > partnerCode assigned: '{Value}'", partnerCode);

            collection.TryGetValue("orderId", out var uniqueOrderId); callbackData.OrderId = uniqueOrderId; // uniqueAttemptId
            _logger.LogInformation(" > orderId assigned: '{Value}'", uniqueOrderId);

            collection.TryGetValue("requestId", out var uniqueRequestId); callbackData.RequestId = uniqueRequestId; // uniqueAttemptId
            _logger.LogInformation(" > requestId assigned: '{Value}'", uniqueRequestId);

            collection.TryGetValue("orderInfo", out var orderInfo); callbackData.OrderInfo = orderInfo;
            _logger.LogInformation(" > orderInfo assigned: '{Value}'", orderInfo);

            collection.TryGetValue("orderType", out var orderType); callbackData.OrderType = orderType;
            _logger.LogInformation(" > orderType assigned: '{Value}'", orderType);

            collection.TryGetValue("transId", out var transId); callbackData.TransId = transId;
            _logger.LogInformation(" > transId assigned: '{Value}'", transId);

            collection.TryGetValue("message", out var message); callbackData.Message = message;
            _logger.LogInformation(" > message assigned: '{Value}'", message);

            collection.TryGetValue("payType", out var payType); callbackData.PayType = payType;
            _logger.LogInformation(" > payType assigned: '{Value}'", payType);

            collection.TryGetValue("extraData", out var extraDataBase64); callbackData.ExtraData = extraDataBase64; // extraData gốc
            _logger.LogInformation(" > extraData assigned: '{Value}'", extraDataBase64);

            collection.TryGetValue("accessKey", out var accessKey); callbackData.AccessKey = accessKey; // Lấy AccessKey nếu MoMo gửi về
            _logger.LogInformation(" > accessKey assigned: '{Value}'", accessKey);

            collection.TryGetValue("localMessage", out var localMessage); callbackData.LocalMessage = localMessage; // Lấy LocalMessage nếu MoMo gửi về
            _logger.LogInformation(" > localMessage assigned: '{Value}'", localMessage);

            // Parse Amount (Decimal)
            if (collection.TryGetValue("amount", out var amountStr) &&
                decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var amountDec))
            {
                callbackData.Amount = amountDec;
                _logger.LogInformation(" > amount assigned: {Value}", amountDec);
            }
            else
            {
                _logger.LogWarning("Could not parse 'amount' from query string: {AmountStr}", amountStr);
                parseError = true; // Đánh dấu lỗi nếu không parse được Amount
            }

            // --- SỬA LOGIC PARSE RESULTCODE/ERRORCODE ---
            bool resultCodeParsed = false;
            int resultCodeValue = -1; // Giá trị mặc định nếu không parse được gì
                                      // Khởi tạo các biến StringValues
            Microsoft.Extensions.Primitives.StringValues resultCodeValues = default;
            Microsoft.Extensions.Primitives.StringValues errorCodeValues = default;

            // 1. Ưu tiên đọc 'resultCode' trước
            // Sử dụng 'out StringValues'
            if (collection.TryGetValue("resultCode", out resultCodeValues) &&
                // Lấy giá trị string từ StringValues để parse
                int.TryParse(resultCodeValues.ToString(), out resultCodeValue))
            {
                callbackData.ResultCode = resultCodeValue;
                _logger.LogInformation(" > ResultCode (from resultCode) assigned: {Value}", resultCodeValue);
                resultCodeParsed = true;
            }
            // 2. Nếu không có 'resultCode', thử đọc 'errorCode'
            // Sử dụng 'out StringValues'
            else if (collection.TryGetValue("errorCode", out errorCodeValues) &&
                     // Lấy giá trị string từ StringValues để parse
                     int.TryParse(errorCodeValues.ToString(), out resultCodeValue)) // Vẫn gán vào biến resultCodeValue
            {
                callbackData.ResultCode = resultCodeValue;
                _logger.LogInformation(" > ResultCode (from errorCode) assigned: {Value}", resultCodeValue);
                resultCodeParsed = true;
            }

            // 3. Chỉ đánh dấu lỗi parse nếu CẢ HAI đều không đọc được
            if (!resultCodeParsed)
            {
                // Log giá trị string từ StringValues (sẽ là null nếu key không tồn tại)
                _logger.LogWarning("Could not parse 'resultCode' or 'errorCode' from query string. resultCodeValues='{ResultCodeValues}', errorCodeValues='{ErrorCodeValues}'", resultCodeValues.ToString(), errorCodeValues.ToString());
                callbackData.ResultCode = 999; // Gán mã lỗi nội bộ
                parseError = true;
            }
            // --- KẾT THÚC SỬA LOGIC PARSE ---

            // Parse ResponseTime (lưu string gốc)
            collection.TryGetValue("responseTime", out var responseTimeString);
            callbackData.ResponseTimeString = responseTimeString;
            _logger.LogInformation(" > ResponseTimeString assigned: '{Value}'", responseTimeString);

            // Đọc và gán Signature
            collection.TryGetValue("signature", out var signature);
            callbackData.Signature = signature;
            _logger.LogInformation(" > signature assigned: '{Value}'", signature);

            // Kiểm tra signature ngay sau khi gán (quan trọng)
            if (string.IsNullOrEmpty(callbackData.Signature))
            {
                _logger.LogWarning("!!! callbackData.Signature is NULL or EMPTY right after assignment in PaymentCallBack !!!");
                // Nếu signature là bắt buộc để xử lý tiếp, nên coi đây là lỗi parse
                parseError = true;
                _logger.LogError("Signature is missing from MoMo callback, cannot proceed."); // Thêm log lỗi rõ ràng
                ViewBag.Message = "Dữ liệu MoMo trả về không hợp lệ (thiếu chữ ký).";
                return View("PaymentFailure"); // Dừng lại sớm nếu thiếu chữ ký
            }
            // --- KẾT THÚC PHẦN ĐỌC VÀ GÁN GIÁ TRỊ ---


            // --- KIỂM TRA LỖI PARSE TỔNG QUÁT ---
            if (parseError) // Chỉ lỗi nếu Amount hoặc cả resultCode/errorCode hoặc signature bị lỗi parse/thiếu
            {
                _logger.LogError("Failed to parse critical data (Amount, ResultCode/ErrorCode, or Signature) from MoMo callback query string.");
                ViewBag.Message = "Dữ liệu MoMo trả về không hợp lệ hoặc bị thiếu.";
                return View("PaymentFailure");
            }

            // --- DECODE extraData ĐỂ LẤY bookingId GỐC ---
            long bookingId = 0;
            if (!string.IsNullOrEmpty(extraDataBase64))
            {
                try
                {
                    var extraDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(extraDataBase64));
                    var extraDataObject = System.Text.Json.JsonSerializer.Deserialize<ExtraDataModel>(extraDataJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // Thêm option nếu cần
                    if (extraDataObject != null)
                    {
                        bookingId = extraDataObject.bookingId;
                        _logger.LogInformation("Decoded bookingId {BookingId} from extraData", bookingId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize extraData JSON: {ExtraDataJson}", extraDataJson);
                        throw new FormatException("Cannot deserialize extraData JSON."); // Ném lỗi để vào catch
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to decode or parse extraData: {ExtraDataBase64}", extraDataBase64);
                    ViewBag.Message = "Dữ liệu MoMo trả về không hợp lệ (extraData).";
                    return View("PaymentFailure");
                }
            }
            else
            {
                _logger.LogWarning("extraData is missing in MoMo callback.");
                ViewBag.Message = "Dữ liệu MoMo trả về bị thiếu (extraData).";
                return View("PaymentFailure");
            }

            // Kiểm tra bookingId hợp lệ
            if (bookingId <= 0)
            {
                _logger.LogError("Invalid BookingId ({BookingId}) obtained from extraData.", bookingId);
                ViewBag.Message = "Mã đơn hàng gốc không hợp lệ.";
                return View("PaymentFailure");
            }

            // --- XỬ LÝ LOGIC CHÍNH ---
            // Kiểm tra mã lỗi MoMo trả về SAU KHI đã parse thành công
            if (callbackData.ResultCode != 0) // Kiểm tra ResultCode đã parse được
            {
                _logger.LogWarning("MoMo payment failed via Return URL. ResultCode: {ResultCode}, Message: {Message}", callbackData.ResultCode, callbackData.Message);
                // Cập nhật trạng thái lỗi vào DB nếu cần
                // await UpdateBookingStatusOnError(bookingId, "Failed_MoMo_Callback", $"MoMo Error {callbackData.ResultCode}: {callbackData.Message}");
                ViewBag.Message = "Thanh toán thất bại theo thông báo từ MoMo: " + callbackData.Message;
                ViewBag.OrderId = callbackData.OrderId; // Hiển thị Attempt ID
                                                        // ViewBag.BookingId = bookingId; // Hiển thị Booking ID gốc
                return View("PaymentFailure");
            }

            // Validate chữ ký (Bây giờ callbackData đã đầy đủ)
            bool isSignatureValid = _momoService.ValidateReturnUrlSignature(callbackData);
            _logger.LogInformation("Return URL Signature validation result: {IsValid} for BookingId: {BookingId}", isSignatureValid, bookingId);

            // --- TÌM BOOKING BẰNG bookingId GỐC ---
            var booking = await _dbContext.Bookings
                                            .Include(b => b.Tour)
                                            .Include(b => b.Customer)
                                          .Include(b => b.Payment) // Include Payment để cập nhật
                                          .FirstOrDefaultAsync(b => b.BookingID == bookingId); // <<< Dùng bookingId từ extraData

            if (booking == null)
            {
                _logger.LogError("Booking not found for ID {BookingId} from Return URL callback.", bookingId);
                ViewBag.Message = "Không tìm thấy đơn hàng trong hệ thống.";
                ViewBag.IsSignatureValid = isSignatureValid; // Vẫn báo kết quả check sig
                return View("PaymentFailure");
            }

            if (isSignatureValid)
            {
                EmailRequest request = new EmailRequest();
                _logger.LogInformation("Processing successful payment confirmation for BookingId {BookingId}", bookingId);

                TourGroup? tourGroup = await _tourGroupRepository.FindByTourAndDateAsync(booking.TourID, booking.DepartureDate);
                int totalNewGuests = booking.NumberOfAdults + booking.NumberOfChildren;
                if (tourGroup != null)
                {
                    tourGroup.TotalGuests += totalNewGuests;
                }
                await _tourGroupRepository.UpdateAsync(tourGroup);
                // Chỉ cập nhật nếu chưa Paid để tránh xử lý lại
                if (booking.Status != "Paid")
                { 
                    request.htmlbody = MailUtil.CreateBooking(booking);
                    _emailSerivce.SendEmail("trangtran.170204@gmail.com", "TOUREST: Xác nhận đặt tour thành công", request.htmlbody);
                   
                    booking.Status = "Paid";
                    booking.TourGroupID = tourGroup.TourGroupID;
                    var paymentTime = callbackData.ResponseTimeConvertedUtc ?? DateTime.UtcNow; // Lấy thời gian chuẩn

                    // Tạo hoặc cập nhật Payment record
                    if (booking.Payment == null)
                    {
                        var payment = new Payment
                        {
                            BookingID = booking.BookingID,
                            Amount = (int)callbackData.Amount, // Hoặc decimal tùy kiểu DB
                            PaymentDate = paymentTime,
                            PaymentMethod = "MoMo",
                            TransactionID = callbackData.TransId, // Lưu mã giao dịch MoMo
                            Status = "Completed",
                            // PaymentNotes = $"Paid via MoMo Return URL. AttemptID: {callbackData.OrderId}" // Ghi chú thêm
                        };
                        _dbContext.Payments.Add(payment);
                        await _dbContext.SaveChangesAsync(); // Lưu Payment trước
                        booking.PaymentID = payment.PaymentID; // Gán FK
                        await _dbContext.SaveChangesAsync(); // Lưu lại Booking với PaymentID
                        _logger.LogInformation("Created new Payment record for BookingId {BookingId}", bookingId);
                    }
                    else
                    {
                        booking.Payment.Amount = (int)callbackData.Amount; // Hoặc decimal
                        booking.Payment.PaymentDate = paymentTime;
                        booking.Payment.PaymentMethod = "MoMo";
                        booking.Payment.TransactionID = callbackData.TransId;
                        booking.Payment.Status = "Completed";
                        // booking.Payment.PaymentNotes = $"Payment updated via MoMo Return URL. AttemptID: {callbackData.OrderId}";
                        await _dbContext.SaveChangesAsync(); // Lưu thay đổi Payment và Booking Status
                        _logger.LogInformation("Updated existing Payment record for BookingId {BookingId}", bookingId);
                    }
                }
                else
                {
                    _logger.LogInformation("Booking {BookingId} was already Paid when Return URL callback received.", booking.BookingID);
                }

                ViewBag.Message = "Thanh toán thành công!";
                ViewBag.OrderId = callbackData.OrderId; // Hiển thị Attempt ID (uniqueOrderId)
                // ViewBag.BookingId = bookingId; // Hiển thị Booking ID gốc nếu muốn
                ViewBag.Amount = callbackData.Amount;
                ViewBag.TransactionId = callbackData.TransId;
                ViewBag.IsSignatureValid = true;
                return View("PaymentSuccess");
            }
            else // Chữ ký không hợp lệ
            {
                _logger.LogWarning("Invalid signature for Return URL payment. BookingId: {BookingId}", bookingId);
                // Cập nhật trạng thái lỗi vào DB nếu cần
                // await UpdateBookingStatusOnError(bookingId, "Failed_Signature_Callback", "Invalid MoMo signature on Return URL.");
                ViewBag.Message = "Chữ ký giao dịch không hợp lệ.";
                ViewBag.OrderId = callbackData.OrderId; // Hiển thị Attempt ID
                ViewBag.IsSignatureValid = false;
                return View("PaymentFailure");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MoMo payment callback (Return URL)");
            ViewBag.Message = "Đã xảy ra lỗi hệ thống khi xử lý kết quả thanh toán.";
            return View("PaymentFailure");
        }
    }


    // --- PaymentNotify (POST - Xử lý IPN) ---
    [HttpPost]
    public async Task<IActionResult> PaymentNotify([FromBody] MomoCallbackViewModel result)
    {
        Booking booking = null;
        long bookingId = 0;

        try
        {
            // ... (Log IPN data như cũ) ...

            if (result == null || string.IsNullOrEmpty(result.OrderId) || string.IsNullOrEmpty(result.ExtraData)) // Kiểm tra cả ExtraData
            {
                _logger.LogWarning("IPN request body is null or OrderId/ExtraData is missing.");
                return Ok(new { RspCode = "99", Message = "Invalid data" });
            }

            // --- DECODE extraData ĐỂ LẤY bookingId GỐC ---
            try
            {
                var extraDataJson = Encoding.UTF8.GetString(Convert.FromBase64String(result.ExtraData));
                var extraDataObject = System.Text.Json.JsonSerializer.Deserialize<ExtraDataModel>(extraDataJson); // Cần class ExtraDataModel
                if (extraDataObject != null)
                {
                    bookingId = extraDataObject.bookingId;
                    _logger.LogInformation("IPN: Decoded bookingId {BookingId} from extraData for MoMo OrderId {MomoOrderId}", bookingId, result.OrderId);
                }
                else
                {
                    _logger.LogWarning("IPN: Failed to deserialize extraData JSON: {ExtraDataJson}", extraDataJson);
                    return Ok(new { RspCode = "99", Message = "Invalid extraData format" }); // Lỗi dữ liệu
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IPN: Failed to decode or parse extraData: {ExtraDataBase64}", result.ExtraData);
                return Ok(new { RspCode = "99", Message = "Invalid extraData" }); // Lỗi dữ liệu
            }

            // Kiểm tra bookingId hợp lệ
            if (bookingId <= 0)
            {
                _logger.LogError("IPN: Invalid BookingId ({BookingId}) obtained from extraData.", bookingId);
                return Ok(new { RspCode = "97", Message = "Invalid BookingId in extraData" }); // Có thể dùng mã lỗi này
            }

            // Validate chữ ký (Hàm validate dùng các trường gốc MoMo gửi về, gồm uniqueOrderId và extraDataBase64)
            bool isSignatureValid = _momoService.ValidateIpnSignature(result);
            if (!isSignatureValid)
            {
                _logger.LogWarning("IPN Signature is invalid for BookingID: {BookingId} (MoMo OrderId: {MomoOrderId})", bookingId, result.OrderId);
                return Ok(new { RspCode = "98", Message = "Invalid signature" });
            }

            _logger.LogInformation("IPN Signature validated successfully for BookingID: {BookingId}", bookingId);

            // --- TÌM BOOKING BẰNG bookingId GỐC ---
            booking = await _dbContext.Bookings
                                      .Include(b => b.Payment)
                                      .FirstOrDefaultAsync(b => b.BookingID == bookingId); // <<< Dùng bookingId từ extraData

            // ... (Phần còn lại xử lý booking null (RspCode 02), kiểm tra Status Paid (RspCode 00), kiểm tra Amount (RspCode 04), cập nhật DB, xử lý lỗi MoMo (ResultCode != 0) giữ nguyên như code trước) ...
            // ... (Chỉ cần đảm bảo bạn đang dùng biến 'booking' đã tìm được bằng bookingId gốc) ...
        }
        catch (Exception ex)
        {
            // ... (Log lỗi và return RspCode 99 như cũ) ...
            _logger.LogError(ex, "IPN: General Exception during processing for Parsed BookingID: {BookingId}", bookingId);
            if (booking != null && booking.Status != "Paid") { try { booking.Status = "Failed_SystemError_IPN"; await _dbContext.SaveChangesAsync(); } catch { /* Ignore nested */ } }
            return Ok(new { RspCode = "99", Message = "Internal Server Error" });
        }
        return Ok(new { RspCode = "99", Message = "Unknown Error" }); // Return mặc định
    }

    // Optional Helper Method to update status on error consistently
    // private async Task UpdateBookingStatusOnError(string orderId, string errorStatus, string errorMessage)
    // {
    //     if (long.TryParse(orderId, out long bookingId))
    //     {
    //          var bookingToUpdate = await _dbContext.Bookings.Include(b => b.Payment).FirstOrDefaultAsync(b => b.BookingID == bookingId);
    //          // Chỉ cập nhật nếu chưa Paid và trạng thái hiện tại cho phép
    //          if (bookingToUpdate != null && bookingToUpdate.Status != "Paid")
    //          {
    //              bookingToUpdate.Status = errorStatus;
    //              if (bookingToUpdate.Payment != null) {
    //                   bookingToUpdate.Payment.Status = "Failed";
    //                   bookingToUpdate.Payment.PaymentNotes = errorMessage;
    //              } else {
    //                   // Có thể tạo Payment mới với status Failed nếu muốn
    //              }
    //              await _dbContext.SaveChangesAsync();
    //              _logger.LogWarning("Updated Booking {BookingId} status to {ErrorStatus} due to: {ErrorMessage}", bookingId, errorStatus, errorMessage);
    //          }
    //     } else {
    //          _logger.LogWarning("Could not parse OrderId {OrderId} to update status on error: {ErrorMessage}", orderId, errorMessage);
    //     }
    // }
    private class ExtraDataModel
    {
        public long bookingId { get; set; }
        // Thêm các trường khác nếu bạn encode chúng vào extraData
    }
}

// --- Nhớ kiểm tra các Model khác ---
// MomoFormDataModel, Payment, Booking, Customer, Tour,... phải được định nghĩa đúng.
// Ví dụ MomoFormDataModel:
public class MomoFormDataModel
{
    public string OrderId { get; set; } // Chính là BookingID gửi từ form
    public decimal Amount { get; set; }
    // Thêm các trường khác nếu form gửi lên
}
