using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class NotificationRepository : INotificationRepository

    {
        private readonly ApplicationDbContext _dbcontext;

        public NotificationRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<int> SendMessageToCustomer(Notification notificationCus)
        {
            await _dbcontext.Notifications.AddAsync(notificationCus);
            await _dbcontext.SaveChangesAsync();

            return 1;
        }
    }
}
