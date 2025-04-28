using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace Tourest.ViewModels.Booking
{
    public class MomoCallbackViewModel
    {
        // --- Các trường chung cho cả Callback và IPN ---

        // Không dùng [FromQuery]/[FromForm] ở đây để ViewModel linh hoạt hơn
        // Việc binding sẽ do Controller xử lý (từ Query hoặc Body)

        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public decimal Amount { get; set; } // Sử dụng decimal
        public string OrderInfo { get; set; }
        public string OrderType { get; set; }

        [JsonProperty("transId")] // Đảm bảo map đúng từ JSON IPN nếu tên khác
        public string TransId { get; set; } // Có thể là long hoặc string tùy MoMo trả về

        public string Message { get; set; }
        public string PayType { get; set; }
        public string ExtraData { get; set; }
        public string Signature { get; set; }

        // --- Trường có thể khác nhau hoặc chỉ có ở một loại response ---

        // Map từ cả "errorCode" (returnUrl) và "resultCode" (IPN)
        // Controller sẽ cần gán giá trị này tùy nguồn dữ liệu
        [JsonProperty("resultCode")] // Ưu tiên map từ 'resultCode' trong JSON IPN
        public int ResultCode { get; set; } // Đổi tên và kiểu thành int (thường là 0 thành công)

        // ResponseTime gốc dạng Unix ms (cho IPN) hoặc string (cho Callback?)
        // Controller sẽ gán giá trị này
        // Không nên là DateTime ở đây vì IPN là long
        public long? ResponseTimeRaw { get; set; } // Lưu Unix timestamp nếu có
        public string ResponseTimeString { get; set; } // Lưu dạng string nếu là callback và không parse được ngay

        // Thuộc tính tính toán để lấy DateTime (UTC) nếu có thể
        [JsonIgnore] // Không serialize/deserialize
        public DateTime? ResponseTimeConvertedUtc
        {
            get
            {
                if (ResponseTimeRaw.HasValue)
                {
                    try { return DateTimeOffset.FromUnixTimeMilliseconds(ResponseTimeRaw.Value).UtcDateTime; } catch { /* ignore */ }
                }
                if (!string.IsNullOrEmpty(ResponseTimeString))
                {
                    // Thử parse string theo định dạng MoMo hay dùng cho callback (nếu biết) hoặc định dạng chuẩn
                    if (DateTime.TryParse(ResponseTimeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt))
                    {
                        return dt;
                    }
                    // Thử định dạng khác nếu cần: ví dụ "yyyy-MM-dd HH:mm:ss"
                    if (DateTime.TryParseExact(ResponseTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dt))
                    {
                        return dt;
                    }
                }
                return null;
            }
        }

        // Có thể có trong returnUrl (query string) nhưng không có trong IPN (JSON)
        public string AccessKey { get; set; }
        public string LocalMessage { get; set; }
    }
}
