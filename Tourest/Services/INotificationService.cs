using Tourest.ViewModels.NotificationView;

namespace Tourest.Services
{
    public interface INotificationService
    {
        Task<string> SendingMessage(List<int> listUserID, NotificationViewModel notificationView);  // type == RelatedEntityID
    }
}
