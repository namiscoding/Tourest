namespace Tourest.ViewModels.Admin
{
    public class AdminCustomerDetailsViewModel : AdminCustomerViewModel // Kế thừa nếu tiện
    {
        public string? Address { get; set; }
        // Thêm các thông tin khác cần hiển thị chi tiết
        // public int BookingCount { get; set; }
    }
}
