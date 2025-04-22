using Microsoft.AspNetCore.SignalR;
using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Util;
using Tourest.ViewModels.NotificationView;

namespace Tourest.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public NotificationService(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        public async Task<string> SendingMessage(List<int> listUserID, NotificationViewModel notificationView)

        {
            List<int> idError = new List<int>();
            int count = 0;
            foreach (int uid in listUserID)
            {

                var newNotificationView = new NotificationViewModel
                {
                    RecipientUserID = uid,
                    SenderUserID = notificationView.SenderUserID,
                    Type = notificationView.Type,
                    Title = notificationView.Title,
                    Content = notificationView.Content,
                    RelatedEntityID = notificationView.RelatedEntityID,
                    RelatedEntityType = notificationView.RelatedEntityType,
                    Timestamp = DateTime.Now,
                    IsRead = false,
                    ActionUrl = notificationView.ActionUrl
                };
                Notification notificationEntity = NotificationMapper.toNotification(newNotificationView);
                await _hubContext.Clients.Group($"toROle {uid}")
               .SendAsync("ReceiveNotification", newNotificationView.Title);

                count += await notificationRepository.SendMessageToCustomer(notificationEntity);


            }

            return "Sending Successfull";
        }


    }
}
