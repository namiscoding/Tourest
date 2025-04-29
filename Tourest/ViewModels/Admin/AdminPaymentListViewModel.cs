using System.ComponentModel.DataAnnotations;

namespace Tourest.ViewModels.Admin
{
    public class AdminPaymentListViewModel
    {
        [Display(Name = "Payment ID")]
        public int PaymentId { get; set; }

        [Display(Name = "Booking ID")]
        public int BookingId { get; set; }

        [Display(Name = "Khách hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Tên Tour")]
        public string TourName { get; set; } = string.Empty;

        [Display(Name = "Số tiền")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Amount { get; set; }

        [Display(Name = "Phương thức")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Mã GD (Cổng TT)")]
        public string? TransactionId { get; set; }

        [Display(Name = "Ngày TT")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime PaymentDate { get; set; }

    }
}
