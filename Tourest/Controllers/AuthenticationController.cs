using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.Util;
using Tourest.ViewModels.Account;

namespace Tourest.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAccountService _accountService;
        public AuthenticationController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IAccountService accountService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
        }

        // GET: /Account/Login

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
    
            var model = new AuthenticationViewModel();
            return View(model);
        }

        //private readonly IUserService _userService;
        [HttpPost]
        public async Task<IActionResult> Login(AuthenticationViewModel model)
        {
            Console.WriteLine(model.Register.ToString());
            ViewData["ReturnUrl"] = model.Register.ReturnUrl;
            User? result = await _accountService.CheckEmailAsync(model.Login.Email);
            if (result == null)     
            {
                // Xử lý khi email đã tồn tại (hoặc có thông báo lỗi)
                TempData["Message"] = "Email  is wrong";
                TempData["ActiveTab"] = "signin";
                return RedirectToAction("Login", "Authentication"); // hoặc return RedirectToAction, tùy bạn
            }
            if (!BCrypt.Net.BCrypt.Verify(model.Login.PasswordHash, result.Account.PasswordHash))
            {
                TempData["Message"] = "password is wrong";
                TempData["ActiveTab"] = "signin";
                return RedirectToAction("Login", "Authentication");
            }

                UserViewModel CurrentAccount = AccountMapper.UserToUserViewModel(result);
            Console.WriteLine(CurrentAccount.ToString());
            if (CurrentAccount != null)
            {
                HttpContext.Session.SetObject("CurrentAccount", CurrentAccount);

            }
            return RedirectToAction("Index", "Tours"); }
    


        [HttpPost]
        public async Task<IActionResult> Register(AuthenticationViewModel model)
        {
            Console.WriteLine(model.Register.ToString());
            ViewData["ReturnUrl"] = model.Register.ReturnUrl;
            User? result = await _accountService.CheckEmailAsync(model.Register.Email);
          
            Console.WriteLine(result);

            if (result != null)
            {
                // Xử lý khi email đã tồn tại (hoặc có thông báo lỗi)
                TempData["Message"] ="Email exist";
                TempData["ActiveTab"] = "signup";
                return RedirectToAction("Login", "Authentication"); // hoặc return RedirectToAction, tùy bạn
            }

            // Nếu null → tức là email chưa tồn tại → tiếp tục xử lý đăng ký

            UserViewModel CurrentAccount =await _accountService.RegisterAsync(model.Register);
            if (CurrentAccount != null)
            {
                HttpContext.Session.SetObject("CurrentAccount", CurrentAccount);

            }
            return RedirectToAction("Index","Tours"); }



    }
}
