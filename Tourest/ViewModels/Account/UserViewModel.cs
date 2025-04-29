using System.ComponentModel.DataAnnotations;
using Tourest.Data.Entities;

namespace Tourest.ViewModels.Account
{
    public class UserViewModel
    {

        public int UserID { get; set; }
        [Required(ErrorMessage = "FullName cannot be empty.")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email cannot be empty.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|net|org|vn)$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number cannot be empty.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits (0-9).")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
   
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Ảnh đại diện mới (Chọn file nếu muốn thay đổi)")]
        public IFormFile? ProfilePictureFile { get; set; } // Nhận file upload mới

        // Navigation Properties
        public virtual AccountViewModel? Account { get; set; }
        public virtual Tourest.Data.Entities.TourGuide? TourGuide { get; set; }
        // Sử dụng namespace đầy đủ của lớp Entity Booking
        public virtual ICollection<Tourest.Data.Entities.Booking> Bookings { get; set; } = new List<Tourest.Data.Entities.Booking>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Tourest.Data.Entities.SupportRequest> SubmittedSupportRequests { get; set; } = new List<Tourest.Data.Entities.SupportRequest>();
        public virtual ICollection<Tourest.Data.Entities.SupportRequest> HandledSupportRequests { get; set; } = new List<Tourest.Data.Entities.SupportRequest>();
        public virtual ICollection<TourGuideAssignment> TourGuideAssignments { get; set; } = new List<TourGuideAssignment>(); // As Guide
        public virtual ICollection<TourGuideAssignment> TourManagerAssignments { get; set; } = new List<TourGuideAssignment>(); // As Manager
        public virtual ICollection<Tourest.Data.Entities.TourGuideRating> TourGuideRatingsReceived { get; set; } = new List<Tourest.Data.Entities.TourGuideRating>();
        public virtual ICollection<TourAuditLog> TourAuditLogsPerformed { get; set; } = new List<TourAuditLog>();
        public virtual ICollection<Notification> NotificationsReceived { get; set; } = new List<Notification>();
        public virtual ICollection<Notification> NotificationsSent { get; set; } = new List<Notification>();
        public override string ToString()
        {
            return $"UserID: {UserID}, " +
                   $"FullName: {FullName}, " +
                   $"Email: {Email}, " +
                   $"PhoneNumber: {PhoneNumber}, " +
                   $"Address: {Address}, " +
                   $"ProfilePictureUrl: {ProfilePictureUrl}, " +
                   $"RegistrationDate: {RegistrationDate:yyyy-MM-dd HH:mm:ss}, " +
                   $"IsActive: {IsActive}";
        }

    }
}
