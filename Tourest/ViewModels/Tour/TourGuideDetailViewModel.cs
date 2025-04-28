namespace Tourest.ViewModels.Tour
{
    public class TourGuideDetailViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Experience { get; set; }
        public string? Languages { get; set; }
        public string? Specializations { get; set; }
        public int MaxGroupSizeCapacity { get; set; }
        public decimal Rating { get; set; }
        public string? ProfilePictureUri { get; set; }
    }

}
