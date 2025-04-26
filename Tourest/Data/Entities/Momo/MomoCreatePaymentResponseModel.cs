namespace Tourest.Data.Entities.Momo
{
    public class MomoCreatePaymentResponseModel
    {
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public long Amount { get; set; } // Kiểu long
        public long ResponseTime { get; set; } // Kiểu long (Unix ms timestamp)
        public string Message { get; set; } // Kiểu string
        public int ResultCode { get; set; } // Kiểu int
        public string PayUrl { get; set; } // Kiểu string
                                           // Thêm các trường khác nếu MoMo trả về và bạn cần (deeplink, qrCodeUrl...)
    }
}
