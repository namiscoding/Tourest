using Microsoft.AspNetCore.Mvc;
using Tourest.ViewModels.Account;

namespace Tourest.Controllers
{
    public class ProfileController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new UserViewModel();
            return View(model);
        }
    }
}
