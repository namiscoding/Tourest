using Tourest.ViewModels.Account;

namespace Tourest.ViewModels.NotificationView
{
    public class NotificationViewModel
    {
        public int NotificationID { get; set; }
        public int RecipientUserID { get; set; } // FK property
        public int? SenderUserID { get; set; } // FK property (nullable)
        public string Type { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? RelatedEntityID { get; set; }
        public string? RelatedEntityType { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string? ActionUrl { get; set; }

        // Navigation Properties
        public virtual UserViewModel RecipientUser { get; set; } = null!;
        public virtual UserViewModel? SenderUser { get; set; }
    }
}
