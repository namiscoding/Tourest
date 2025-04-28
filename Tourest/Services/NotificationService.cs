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





        public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
        {
            this.notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task<string> SendingMessage(int userId, NotificationViewModel notificationView)


        {
         
            int count = 0;

            var newNotificationView = new NotificationViewModel
            {
                RecipientUserID = userId,
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


            count += await notificationRepository.SendMessageToCustomer(notificationEntity);
            return "Sending Successfull";

        }

          
        } 
}


    

