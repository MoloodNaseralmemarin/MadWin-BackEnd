using MadWin.Application.DTOs.Account;
using MadWin.Application.Services;
using MadWin.Core.Common;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shop2City.Core.Services.Permissions;
using Shop2City.WebHost.ViewModels.Account;
using System.Security.Claims;
using System.Xml;


namespace Shop2City.WebHost.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPermissionService _permissionService;

        public AccountController(IUserService userService, ILogger<AccountController> logger, ISmsSenderService smsSenderService, IPasswordHasher passwordHasher, IUserRepository userRepository, IPermissionService permissionService)
        {
            _userService = userService;
            _userRepository = userRepository;
            _logger = logger;
            _smsSenderService = smsSenderService;
            _passwordHasher = passwordHasher;
            _permissionService = permissionService;


        }
        #region Register

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Route("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (await _userService.IsExistCellPhoneAsync(model.CellPhone))
            {
                ModelState.AddModelError("CellPhone", ErrorMessage.InvalidCellPhone);
                return View(model);
            }
            #region Add User
            model.Password = _passwordHasher.Hash(model.Password);
            var user = new User
            {
                Address = model.Address,
                TelPhone=model.TelPhone,
                CellPhone = model.CellPhone,
                FirstName=model.FirstName,
                LastName=model.LastName,
                Password=model.Password,
                UserName=model.CellPhone

            };
            await _userService.CreateUser(user);
            #endregion
            #region welcome SMS
            var smsSent = await _smsSenderService.SendWelcomeSmsAsync(model.CellPhone, model.FullName, 2);
            _logger.LogWarning($"ارسال پیامک به {model.CellPhone} {(smsSent ? "موفقیت‌آمیز" : "ناموفق")} بود.");

            #endregion
            return RedirectToAction("Login");
        }
        #endregion
        #region Login
        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
            {
                return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid during login for {User}", model.userName);
                return View(model);
            }

            try
            {
                var result = await _userService.LoginAsync(new LoginResultDto
                {
                    UserName = model.userName,
                    Password = model.password
                });

                if (result == null)
                {
                    TempData["ToastrError"]="نام کاربری یا رمز عبور نادرست است.";
                    return View(model);
                }

                // تعریف Claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
            new Claim(ClaimTypes.Name, result.FullName)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.rememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                _logger.LogInformation("User {UserId} logged in.", result.Id);

                // بررسی returnUrl اول
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                // بررسی نقش کاربر
                bool isAdmin = await _permissionService.CheckPermissionAsync(1, result.Id);
                if (isAdmin)
                {
                    _logger.LogInformation(
                        $"مدیریت سایت با موفقیت وارد سایت شده است. UserId: {result.Id}");
                    return RedirectToAction("Index", "AdminPanel", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for {User}", model.userName);
                ModelState.AddModelError("", "خطایی در ورود رخ داد.");
                return View(model);
            }
        }
        #endregion
        #region ForgotPassword
        [Route("ForgotPassword")]
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (!ModelState.IsValid)
                return View(forgot);

            //await _userService.ExtractAndSaveLast4DigitsAsync(forgot.UserName);

            TempData["SuccessMessage"] = "کلمه عبور جدید به شماره موبایل ثبت ‌شده شما پیامک شد.";

            return RedirectToAction("Login");
        }
        #endregion
        
        #region Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Cart");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }
        #endregion
    }
}