using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Services;
using Tourest.Util;
using Tourest.ViewModels.Account;

namespace Tourest.Controllers
{
    public class ProfileController : Controller
    {

        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        
    

        public ProfileController(IUserService userService, IAccountService accountService, IUserRepository userRepository)
        {
            _userService = userService;
            _accountService = accountService;
            _userRepository = userRepository;
        }


        [HttpGet]
        public IActionResult Index(int id)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(id);
            Console.WriteLine(loggedInUserId);
            if (loggedInUserId == null || loggedInUserId != id.ToString())
            {
                return RedirectToAction("Login", "Authentication");

            }
            var userJson = HttpContext.Session.GetString("CurrentAccount");
            var userObj = JsonConvert.DeserializeObject<UserViewModel>(userJson);
            Console.WriteLine(userObj);
            return View(userObj);
        }

        [Route("/ChangePassword/{id}")]
        [HttpGet]
        public IActionResult ChangePassword(int id)
        {
            ChangePasswordModel change = new ChangePasswordModel();
            change.UserID = id;
            return View(change);
        }
        [HttpPost]   
        
        public async Task<IActionResult> ActionChangePassword(ChangePasswordModel model)
        {
            
            Console.WriteLine(model.toString());
            bool status = await _accountService.UpdatePassword(model.UserID, model.ConfirmNewPassword,model.CurrentPassword);
            if (status == true)
            {
                 TempData["Message"] = "update successfull";
            }
            return RedirectToAction("Index", new { id = model.UserID });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userViewModel, string ConfirmPassword, string emailCurrent)
        {
            User? result = await _accountService.CheckEmailAsync(emailCurrent);
            Console.WriteLine(result);

            
            var updateResult = await _userService.UpdateCustomerByAdminAsync(AccountMapper.ConvertoEdit(userViewModel));
            if (!updateResult.Success)
            {
                TempData["Message"] = updateResult.ErrorMessage;
                return RedirectToAction("Index", new { id = userViewModel.UserID });
            }
           
            // Lấy lại thông tin mới nhất sau khi cập nhật
            var updatedUser = await _accountService.CheckEmailAsync(userViewModel.Email);
            if (updatedUser != null)
            {
                var updatedUserViewModel = AccountMapper.UserToUserViewModel(updatedUser); // Map sang ViewModel nếu cần
                var updatedUserJson = JsonConvert.SerializeObject(updatedUserViewModel);
                HttpContext.Session.SetString("CurrentAccount", updatedUserJson);
            }

            TempData["Message"] = "Cập nhật thành công.";
            return RedirectToAction("Index", new { id = userViewModel.UserID });
        }

    }
}