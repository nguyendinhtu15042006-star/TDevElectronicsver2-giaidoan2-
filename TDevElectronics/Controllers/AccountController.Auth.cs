using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Models.Enums;
using TDevElectronics.Models.ViewModels;
using TDevElectronics.Services.Interfaces;

namespace TDevElectronics.Controllers
{
    /// <summary>Đăng nhập, đăng ký, đăng xuất của khách hàng (views Account/Login, Account/Register).</summary>
    public partial class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService) => _authService = authService;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole(UserRoles.Customer))
                return RedirectToAction(nameof(Index));
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var (ok, message, user) = await _authService.LoginAsync(model.Email, model.Password);
            if (!ok || user is null)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            if (user.Role != UserRoles.Customer)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản quản trị vui lòng đăng nhập tại trang Quản trị viên.");
                return View(model);
            }

            var id = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id),
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                });

            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction(nameof(Index));
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (ok, message, _) = await _authService.RegisterCustomerAsync(model.FullName, model.Email, model.Password);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }
            return RedirectToAction(nameof(Login));
        }

        // GET cũng được phép vì app.js và link "Sign out" ở sidebar chuyển hướng bằng link
        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}