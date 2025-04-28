using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourest.Data.Entities;
using Tourest.Services;
using Tourest.Util;
using Tourest.ViewModels.Account;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, CurrentAccount.UserID.ToString()), // RẤT QUAN TRỌNG
        new Claim(ClaimTypes.Name, CurrentAccount.FullName),
         new Claim(ClaimTypes.Role, CurrentAccount.Account.Role) // Thêm dòng này
    };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            return RedirectToAction("Index", "Tours"); }



        [HttpPost]
        public async Task<IActionResult> Register(AuthenticationViewModel model)
        {
            Console.WriteLine(model.Register.ToString());
            ViewData["ReturnUrl"] = model.Register.ReturnUrl;
            User? result = await _accountService.CheckEmailAsync(model.Register.Email);
            if (result != null)
            {
                // Xử lý khi email đã tồn tại (hoặc có thông báo lỗi)
                TempData["Message"] = "Email exist";
                TempData["ActiveTab"] = "signup";
                return RedirectToAction("Login", "Authentication"); // hoặc return RedirectToAction, tùy bạn
            }

            // Nếu null → tức là email chưa tồn tại → tiếp tục xử lý đăng ký

            UserViewModel CurrentAccount = await _accountService.RegisterAsync(model.Register);
            if (CurrentAccount != null)
            {
                HttpContext.Session.SetObject("CurrentAccount", CurrentAccount);

                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, CurrentAccount.UserID.ToString()), // RẤT QUAN TRỌNG
        new Claim(ClaimTypes.Name, CurrentAccount.FullName),
                 new Claim(ClaimTypes.Role, CurrentAccount.Account.Role) // Thêm dòng này
    };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            return RedirectToAction("Index", "Tours"); }



        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Kiểm tra định dạng email cơ bản
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
                return false;

            // Kiểm tra email phải kết thúc bằng .com
            return email.EndsWith(".com", StringComparison.OrdinalIgnoreCase);
        }
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            foreach (char c in phoneNumber)
            {
                if (!char.IsDigit(c))
                {
                    return false; // Nếu gặp ký tự không phải số thì sai
                }
            }
            return true;
        }

        public List<string> GetNullPropertyNames(object obj)
        {
            var nullProperties = new List<string>();

            if (obj == null)
                return nullProperties;

            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                // Bỏ qua các trường không cần validate
                if (prop.Name == "ReturnUrl")
                    continue;

                var value = prop.GetValue(obj);
                if (value == null)
                {
                    // Lấy Display Name nếu có, không thì lấy prop.Name
                    var displayAttr = prop.GetCustomAttributes(typeof(DisplayAttribute), true)
                                          .FirstOrDefault() as DisplayAttribute;
                    var fieldName = displayAttr != null ? displayAttr.Name : prop.Name;

                    nullProperties.Add(fieldName);
                }
            }

            return nullProperties;
        }

    } }
