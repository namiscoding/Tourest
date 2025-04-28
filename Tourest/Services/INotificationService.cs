using Tourest.ViewModels.NotificationView;

namespace Tourest.Services
{
    public interface INotificationService
    {
        Task<string> SendingMessage(int userId, NotificationViewModel notificationView);  // type == RelatedEntityID
    }
}
