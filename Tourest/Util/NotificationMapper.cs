using Tourest.Data.Entities;
using Tourest.ViewModels.NotificationView;

namespace Tourest.Util
{
    public class NotificationMapper
    {

        public static Notification toNotification(NotificationViewModel notification)
        {
            return new Notification
            {
                NotificationID = notification.NotificationID,
                RecipientUserID = notification.RecipientUserID,
                SenderUserID = notification.SenderUserID,
                Type = notification.Type,
                Title = notification.Title,
                Content = notification.Content,
                RelatedEntityID = notification.RelatedEntityID,
                RelatedEntityType = notification.RelatedEntityType,
                Timestamp = notification.Timestamp,
                IsRead = notification.IsRead,
                ActionUrl = notification.ActionUrl,
                // Navigation properties thường không ánh xạ ở đây trừ khi cần
                // RecipientUser = new User { ... } nếu cần ánh xạ sâu
                // SenderUser = new User { ... }
            };
        }

    }
}
