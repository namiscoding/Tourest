namespace Tourest.ViewModels.Admin
{
    public class AdminTourManagerDetailsViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public List<AssignmentInfoViewModel> AssignmentsMade { get; set; } = new List<AssignmentInfoViewModel>();
    }
}
