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
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<SupportRequest> SubmittedSupportRequests { get; set; } = new List<SupportRequest>();
        public virtual ICollection<SupportRequest> HandledSupportRequests { get; set; } = new List<SupportRequest>();
        public virtual ICollection<TourGuideAssignment> TourGuideAssignments { get; set; } = new List<TourGuideAssignment>(); // As Guide
        public virtual ICollection<TourGuideAssignment> TourManagerAssignments { get; set; } = new List<TourGuideAssignment>(); // As Manager
        public virtual ICollection<TourGuideRating> TourGuideRatingsReceived { get; set; } = new List<TourGuideRating>();
        public virtual ICollection<TourAuditLog> TourAuditLogsPerformed { get; set; } = new List<TourAuditLog>();
        public virtual ICollection<Notification> NotificationsReceived { get; set; } = new List<Notification>();
        public virtual ICollection<Notification> NotificationsSent { get; set; } = new List<Notification>();

    }
}
