namespace Tourest.ViewModels.Tour
{
    public class TourCustomerViewModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? Address { get; set; }
    }
}
