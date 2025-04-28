using Tourest.Data.Entities;

namespace Tourest.ViewModels.Account
{
    public class UserViewModel
    {

        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }

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
