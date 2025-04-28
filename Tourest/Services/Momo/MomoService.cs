using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tourest.Data.Entities.Momo;
using Tourest.Data.Entities;
using System.Security.Cryptography;
using System.Text;
using RestSharp;
using Tourest.ViewModels.Booking;
using System.Diagnostics;
using System.Globalization;
using Microsoft.CodeAnalysis;
namespace Tourest.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        private readonly ILogger<MomoService> _logger;

        public MomoService(IOptions<MomoOptionModel> options, ILogger<MomoService> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // --- CreatePaymentMomo (Đã sửa, giữ nguyên) ---
        public async Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfoModel model)
        {
            // Lấy thông tin từ model (đã chứa uniqueAttemptId và extraData)
            string uniqueOrderId = model.OrderId;
            string uniqueRequestId = model.RequestId;
            string orderInfo = model.OrderInfo; // Đã có thể chứa BKID gốc
            string extraData = model.ExtraData; // Base64 encoded JSON {"bookingId": ...}

            // --- TẠO CHUỖI rawData THEO ĐÚNG ĐỊNH DẠNG MOMO YÊU CẦU (SẮP XẾP A-Z) ---
            var rawData = $"accessKey={_options.Value.AccessKey}" + // Sắp xếp A-Z
                          $"&amount={model.Amount.ToString(CultureInfo.InvariantCulture)}" + // Giá trị Amount dạng chuỗi
                          $"&extraData={extraData}" + // extraData đã encode Base64
                          $"&ipnUrl={_options.Value.NotifyUrl}" + // Dùng tên ipnUrl, giá trị từ cấu hình NotifyUrl
                          $"&orderId={uniqueOrderId}" +
                          $"&orderInfo={orderInfo}" +
                          $"&partnerCode={_options.Value.PartnerCode}" +
                          $"&redirectUrl={_options.Value.ReturnUrl}" + // Dùng tên redirectUrl, giá trị từ cấu hình ReturnUrl
                          $"&requestId={uniqueRequestId}" +
                          $"&requestType={_options.Value.RequestType}"; // Ví dụ: "captureWallet"

            _logger.LogInformation("Raw data string for signature calculation: [{RawDataString}]", rawData); // Log để kiểm tra


            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            // ... (Phần còn lại của RestClient, requestData giữ nguyên, chỉ cần đảm bảo các trường lấy đúng từ model) ...
            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                // KHÔNG GỬI accessKey trong body nữa
                requestId = uniqueRequestId,
                amount = (long)model.Amount, // <<< ĐỔI KIỂU SANG LONG (Giả định Amount không có phần thập phân)
                                             // Nếu Amount có thể là decimal, bạn cần quy đổi về đơn vị nhỏ nhất (VND) và ép kiểu long.
                                             // Ví dụ: amount = (long)(model.Amount) nếu model.Amount đã là VND.
                orderId = uniqueOrderId,
                orderInfo = orderInfo,
                redirectUrl = _options.Value.ReturnUrl, // <<< ĐỔI TÊN TRƯỜNG (giá trị vẫn lấy từ cấu hình ReturnUrl)
                ipnUrl = _options.Value.NotifyUrl,      // <<< ĐỔI TÊN TRƯỜNG (giá trị vẫn lấy từ cấu hình NotifyUrl)
                extraData = extraData,
                requestType = _options.Value.RequestType, // Giữ nguyên "captureWallet" là đúng
                lang = "vi", // <<< THÊM TRƯỜNG lang (hoặc "en", hoặc lấy từ config nếu muốn)
                signature = signature
            };

            // ... (Gửi request và xử lý response như cũ) ...

            // ... (Phần gọi RestClient và xử lý response giữ nguyên) ...
            // ... (Gửi request và xử lý response như cũ) ...
            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = RestSharp.Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddParameter("application/json", System.Text.Json.JsonSerializer.Serialize(requestData), ParameterType.RequestBody); // Dùng System.Text.Json nếu được
            _logger.LogInformation("Sending MoMo Request (AttemptID: {AttemptId}): {RequestBody}", uniqueOrderId, System.Text.Json.JsonSerializer.Serialize(requestData));
            var response = await client.ExecuteAsync(request);
            // ... (Xử lý response và return như code trước) ...

            _logger.LogInformation("Received MoMo Response Status: {StatusCode}, Content: {Content}", response.StatusCode, response.Content);
            var momoResponse = new MomoCreatePaymentResponseModel();
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    // !!! THỬ DESERIALIZE TRỰC TIẾP !!!
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // <<< THÊM OPTION NÀY
                    };
                    momoResponse = System.Text.Json.JsonSerializer.Deserialize<MomoCreatePaymentResponseModel>(response.Content, options); // <<< Truyền options vào

                    // Kiểm tra ngay sau khi deserialize xem PayUrl có giá trị không
                    if (momoResponse != null && momoResponse.ResultCode == 0 && !string.IsNullOrEmpty(momoResponse.PayUrl))
                    {
                        _logger.LogInformation(">>> SUCCESSFUL DESERIALIZATION <<< : ResultCode={ResultCode}, Message={Message}, PayUrl={PayUrl}", momoResponse.ResultCode, momoResponse.Message, momoResponse.PayUrl);
                    }
                    else
                    {
                        // Nếu deserialize không lỗi nhưng PayUrl vẫn null hoặc resultCode != 0
                        _logger.LogWarning("Parsed MoMo Response but PayUrl is missing or ResultCode is not 0. Parsed ResultCode={ResultCode}, Parsed Message={Message}, Parsed PayUrl={PayUrl}. Raw Content: {RawContent}",
                                           momoResponse?.ResultCode, momoResponse?.Message, momoResponse?.PayUrl, response.Content);
                        // Đặt lại thông tin lỗi nếu cần thiết
                        if (momoResponse != null)
                        {
                            momoResponse.ResultCode = momoResponse.ResultCode != 0 ? momoResponse.ResultCode : 98; // Mã lỗi tự định nghĩa
                            momoResponse.Message = momoResponse.Message ?? "Parsed successfully but critical data missing (e.g., PayUrl).";
                        }
                        else
                        {
                            momoResponse = new MomoCreatePaymentResponseModel { ResultCode = 99, Message = "Deserialization resulted in null object." };
                        }
                    }
                }
                catch (System.Text.Json.JsonException jsonEx) // Hoặc Newtonsoft.Json.JsonException
                {
                    _logger.LogError(jsonEx, "Failed to parse MoMo JSON response (AttemptID: {AttemptId}): {Content}", model.OrderId, response.Content);
                    momoResponse = new MomoCreatePaymentResponseModel { ResultCode = 99, Message = "Failed to parse MoMo response." };
                }
            }
            else // Lỗi HTTP
            {
                // ... (Xử lý lỗi HTTP như cũ) ...
                momoResponse = new MomoCreatePaymentResponseModel { ResultCode = (int?)response.StatusCode ?? 99, Message = $"MoMo request failed: {response.ErrorMessage ?? response.StatusDescription}" };
            }
            return momoResponse;
        }

        // --- PaymentExecuteAsync (Giữ nguyên nếu bạn không dùng) ---
        // Phương thức này có vẻ không liên quan trực tiếp đến luồng thanh toán chuẩn
        // public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection) { ... }


        // --- ValidateReturnUrlSignature ---
        public bool ValidateReturnUrlSignature(MomoCallbackViewModel callbackData)
        {
            if (callbackData == null || string.IsNullOrEmpty(callbackData.Signature))
            {
                _logger.LogWarning("Return URL callback data or signature is null/empty.");
                return false;
            }

            // Lấy AccessKey và SecretKey từ cấu hình server
            var accessKeyFromConfig = _options.Value.AccessKey;
            var secretKey = _options.Value.SecretKey;

            if (string.IsNullOrEmpty(accessKeyFromConfig) || string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("AccessKey or SecretKey is null or empty in configuration for Return URL validation.");
                return false;
            }

            // --- TẠO CHUỖI RAW HASH THEO ĐÚNG TÀI LIỆU MOMO (SẮP XẾP A-Z) ---
            // Sử dụng các giá trị từ callbackData (do MoMo gửi về) NGOẠI TRỪ accessKey lấy từ config
            var rawHash = $"accessKey={accessKeyFromConfig}" + // Lấy từ config, sắp xếp A-Z
                          $"&amount={callbackData.Amount.ToString(CultureInfo.InvariantCulture)}" + // amount từ callback
                          $"&extraData={callbackData.ExtraData}" + // extraData từ callback
                          $"&message={callbackData.Message}" + // message từ callback
                          $"&orderId={callbackData.OrderId}" + // orderId từ callback (uniqueAttemptId)
                          $"&orderInfo={callbackData.OrderInfo}" + // orderInfo từ callback
                          $"&orderType={callbackData.OrderType}" + // orderType từ callback
                          $"&partnerCode={callbackData.PartnerCode}" + // partnerCode từ callback
                          $"&payType={callbackData.PayType}" + // payType từ callback
                          $"&requestId={callbackData.RequestId}" + // requestId từ callback (uniqueAttemptId)
                          $"&responseTime={callbackData.ResponseTimeString}" + // responseTime dạng string gốc từ callback
                          $"&resultCode={callbackData.ResultCode}" + // resultCode từ callback
                          $"&transId={callbackData.TransId}"; // transId từ callback
                                                              // KHÔNG có localMessage ở đây

            _logger.LogInformation("Raw hash string for RETURN URL validation (Corrected Format): [{RawHash}]", rawHash);
            _logger.LogInformation("Received signature from RETURN URL: [{Signature}]", callbackData.Signature);

            string calculatedSignature = ComputeHmacSha256(rawHash, secretKey);
            _logger.LogInformation("Calculated signature for RETURN URL: [{Signature}]", calculatedSignature);

            bool isValid = string.Equals(calculatedSignature, callbackData.Signature, StringComparison.OrdinalIgnoreCase);
            _logger.LogInformation("Return URL Signature validation result: {IsValid}", isValid);

            return isValid;
        }

        // --- ValidateIpnSignature ---
        public bool ValidateIpnSignature(MomoCallbackViewModel ipnData)
        {
            if (ipnData == null || string.IsNullOrEmpty(ipnData.Signature))
            {
                _logger.LogWarning("IPN data or signature is null/empty.");
                return false;
            }

            var accessKey = _options.Value.AccessKey; // Lấy từ config
            var secretKey = _options.Value.SecretKey; // Lấy từ config

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("AccessKey or SecretKey is null or empty in configuration for IPN validation.");
                return false;
            }

            // --- TẠO CHUỖI RAW HASH CHO IPN THEO ĐÚNG THỨ TỰ MOMO QUY ĐỊNH (Payment API v2) ---
            // !!! QUAN TRỌNG: ĐÂY LÀ THỨ TỰ CHUẨN THƯỜNG THẤY, NHƯNG VẪN CẦN KIỂM TRA LẠI VỚI DOCS CỦA BẠN !!!
            var rawHash = $"accessKey={accessKey}" + // AccessKey từ config
                          $"&amount={ipnData.Amount.ToString(CultureInfo.InvariantCulture)}" + // Dùng InvariantCulture
                          $"&extraData={ipnData.ExtraData}" +
                          $"&message={ipnData.Message}" +
                          $"&orderId={ipnData.OrderId}" +
                          $"&orderInfo={ipnData.OrderInfo}" +
                          $"&orderType={ipnData.OrderType}" +
                          $"&partnerCode={ipnData.PartnerCode}" + // PartnerCode từ IPN data
                          $"&payType={ipnData.PayType}" +
                          $"&requestId={ipnData.RequestId}" +
                          // Dùng giá trị Unix timestamp gốc (long) đã nhận được
                          $"&responseTime={ipnData.ResponseTimeRaw}" + // <<< Dùng giá trị long gốc
                          $"&resultCode={ipnData.ResultCode}" + // <<< Dùng ResultCode (đã map từ JSON)
                          $"&transId={ipnData.TransId}";

            _logger.LogInformation("Raw hash string for IPN validation: [{RawHash}]", rawHash);
            _logger.LogInformation("Received IPN signature: [{Signature}]", ipnData.Signature);

            string calculatedSignature = ComputeHmacSha256(rawHash, secretKey);
            _logger.LogInformation("Calculated IPN signature: [{Signature}]", calculatedSignature);

            bool isValid = string.Equals(calculatedSignature, ipnData.Signature, StringComparison.OrdinalIgnoreCase);
            _logger.LogInformation("IPN Signature validation result: {IsValid}", isValid);

            return isValid;
        }

        // --- ComputeHmacSha256 (Giữ nguyên) ---
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] hashBytes;
            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public class MomoOptionModel
    {
        public string MomoApiUrl { get; set; }
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string RequestType { get; set; } // Ví dụ: "captureWallet", "payWithMethod"
    }


    // Model nhận kết quả tạo link thanh toán (Ví dụ)
    public class MomoCreatePaymentResponseModel
    {
        public string PartnerCode { get; set; }
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public long Amount { get; set; } // MoMo thường trả về long
        public long ResponseTime { get; set; } // Unix timestamp ms
        public string Message { get; set; }
        public int ResultCode { get; set; } // 0 là thành công
        public string PayUrl { get; set; }
        public string DeepLink { get; set; }
        public string QrCodeUrl { get; set; }
        // Thêm các trường khác nếu cần
    }
}

