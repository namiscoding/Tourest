using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin
{
    public class AdminPaymentDetailsViewModel
    {
        [Display(Name = "Payment ID")]
        public int PaymentId { get; set; }

        [Display(Name = "Booking ID")]
        public int BookingId { get; set; }

        [Display(Name = "Số tiền")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Amount { get; set; } // INT

        [Display(Name = "Ngày Thanh toán")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Phương thức")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Mã GD (Cổng TT)")]
        public string? TransactionId { get; set; }

        // Thông tin Booking liên quan
        [Display(Name = "Ngày Khởi hành Tour")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", NullDisplayText = "N/A")]
        public DateTime? BookingDepartureDate { get; set; }

        [Display(Name = "Số khách (Booking)")]
        public int BookingNumberOfGuests { get; set; }

        // Thông tin Khách hàng liên quan
        [Display(Name = "Customer ID")]
        public int? CustomerId { get; set; }

        [Display(Name = "Tên Khách hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Email Khách hàng")]
        public string? CustomerEmail { get; set; }

        // Thông tin Tour liên quan
        [Display(Name = "Tour ID")]
        public int? TourId { get; set; }

        [Display(Name = "Tên Tour")]
        public string TourName { get; set; } = string.Empty;

        // Thêm các trường khác nếu cần
    }
}
