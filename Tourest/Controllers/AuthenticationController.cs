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
using Azure.Core;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.ViewModels.NotificationView;

namespace Tourest.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailserivce;
        private readonly INotificationService _noti;

     

        private readonly ApplicationDbContext _dbcontext;

       
        public AuthenticationController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IAccountService accountService, IEmailService emailserivce, ApplicationDbContext dbcontext, INotificationService noti)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _emailserivce = emailserivce;
            _dbcontext = dbcontext;
            _noti = noti;

        }

        // GET: /Account/Login

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
    
            var model = new AuthenticationViewModel();
            return View(model);
        }

        [HttpGet]
        public IActionResult EnterEmail()
        {

           
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SendLink(string Email)
        {
            Console.WriteLine(Email);
            string token = MailUtil.GenerateResetToken();
            bool AddTokenStatus = await _accountService.AddtokenForgot(token, Email);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            Console.Write(baseUrl);
            var emailContent = MailUtil.CreatEmailForgot(token, baseUrl);
            Console.WriteLine(emailContent);
                
            bool result = _emailserivce.SendEmail(Email, "Did you forget the password ?", MailUtil.CreatEmailForgot(emailContent));
            if (result == true)
            {
                TempData["Message"] = "Send Successfulll";
            }
            return RedirectToAction("EnterEmail", "Authentication");

        }


        [HttpPost]
        [Route("/Authentication/SubmitResetPassword")]
        public async Task<IActionResult> SubmitResetPassword(ResetPasswordModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp.");
                return View("ResetPasswordForm", model);
            }

            var account = await _dbcontext.Accounts.FirstOrDefaultAsync(a => a.PasswordResetToken == model.Token);

            if (account == null || account.ResetTokenExpiration < DateTime.UtcNow)
            {
                return BadRequest("Token không hợp lệ hoặc đã hết hạn!");
            }

            // Hash mật khẩu mới và update
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            // Xóa token reset (để 1 lần dùng thôi)
            account.PasswordResetToken = null;
            account.ResetTokenExpiration = null;

            _dbcontext.Accounts.Update(account);
            await _dbcontext.SaveChangesAsync();
            TempData["ActiveTab"] = "signin";

            var notification = new NotificationViewModel
            {
                RecipientUserID = account.UserID, // ID của user vừa đổi mật khẩu
                SenderUserID = null, // Hệ thống gửi, nên để null hoặc 0
                Type = "Security", // Loại thông báo: bảo mật
                Title = "Đổi mật khẩu thành công",
                Content = "Bạn đã thay đổi mật khẩu của mình thành công. Nếu đây không phải là bạn, hãy liên hệ với bộ phận hỗ trợ ngay lập tức.",
                RelatedEntityID = "1", // Nếu bạn muốn link tới tài khoản có thể gán ID
                RelatedEntityType = "Account", // hoặc gán "Account" nếu thích
                Timestamp = DateTime.UtcNow,
                IsRead = false, // Ban đầu gán false
                ActionUrl = "/Profile/Security" // Link tới trang đổi mật khẩu hoặc bảo mật
            };

            await _noti.SendingMessage(account.UserID, notification);
            return RedirectToAction("Login", "Authentication"); // hoặc báo đổi mật khẩu thành công
        }

        [HttpGet]
        [Route("/Authentication/ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token không hợp lệ!");
            }

            var account = await _dbcontext.Accounts
                                        .FirstOrDefaultAsync(a => a.PasswordResetToken == token);

            if (account == null)
            {
                return BadRequest("Token không tồn tại hoặc đã hết hạn!");
            }

            if (account.ResetTokenExpiration == null || account.ResetTokenExpiration < DateTime.UtcNow)
            {
                return BadRequest("Token đã hết hạn!");
            }

            // Nếu token hợp lệ, trả về view ResetPasswordForm (form nhập mật khẩu mới)
            var model = new ResetPasswordModel
            {
                Token = token // Gửi token xuống view để submit lại
            };

            return View("ResetPasswordForm", model);
        }


        [HttpPost]
        public async Task<IActionResult> GetLink(string token)
        {

            return null;
        } 
        

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {


            return View("AccessDenied");// Nếu bạn có file Views/Authentication/AccessDenied.cshtml

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
            return RedirectToAction("Index","Tours"); }



    }
}
