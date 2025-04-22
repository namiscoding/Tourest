using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface INotificationRepository
    {
        Task<int> SendMessageToCustomer(Notification notificationCus);
    }
}
