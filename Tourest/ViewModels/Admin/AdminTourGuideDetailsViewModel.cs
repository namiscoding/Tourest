namespace Tourest.ViewModels.Admin
{
    public class AdminTourGuideDetailsViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? ProfilePictureUrl { get; set; } // Thêm nếu cần

        // Tour Guide Profile
        public string? ExperienceLevel { get; set; }
        public string? LanguagesSpoken { get; set; }
        public string? Specializations { get; set; }
        public int? MaxCapacity { get; set; }
        public decimal? AverageRating { get; set; }
            
        // Related Data
        public List<AssignmentLedViewModel> AssignmentsLed { get; set; } = new List<AssignmentLedViewModel>();
        public List<RatingReceivedViewModel> RatingsReceived { get; set; } = new List<RatingReceivedViewModel>();
    }
}
