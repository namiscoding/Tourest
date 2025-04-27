namespace Tourest.ViewModels.Booking
{
    public class MomoFormDataModel
    {
        // Các thuộc tính khớp với name của input trong form
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string OrderId { get; set; } // Đây thực chất là BookingId dạng chuỗi
        public string FullName { get; set; } // Tên này đang bị hardcode "Tourest", sẽ lấy từ DB
        public int TourId { get; set; } // Thêm TourId nếu bạn cần dùng trong action
    }
}
