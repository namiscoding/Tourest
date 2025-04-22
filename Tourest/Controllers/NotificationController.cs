using Microsoft.AspNetCore.Mvc;
using Tourest.Services;
using Tourest.Util;
using Tourest.ViewModels.Account;
using Tourest.ViewModels.NotificationView;

namespace Tourest.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        public IActionResult SendNotification()
        {

            var model = new NotificationViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SendNotification(List<string> listId, NotificationViewModel notificationView)
        { 
            //var status = _notificationService.
            var model = new NotificationViewModel();
            return View(model);
        }

       

    }
}

