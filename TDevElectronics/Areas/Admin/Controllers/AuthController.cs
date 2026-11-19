using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TDevElectronics.Models.Enums;
using TDevElectronics.Models.ViewModels;
using TDevElectronics.Services.Interfaces;

namespace TDevElectronics.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IMemoryCache _cache;

        public AuthController(IAuthService authService, IMemoryCache cache)
        {
            _authService = authService;
            _cache = cache;
        }

        // GET: /Admin/Auth/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole(UserRoles.Admin))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Admin/Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message, user) = await _authService.LoginAsync(model.Email, model.Password);
            if (!success || user == null)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            if (user.Role != UserRoles.Admin)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản này không có quyền truy cập khu vực quản trị.");
                return View(model);
            }

            await SignInUserAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        // GET: /Admin/Auth/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? code = null)
        {
            var model = new AdminRegisterViewModel
            {
                InviteCode = code ?? string.Empty
            };
            return View(model);
        }

        // POST: /Admin/Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AdminRegisterViewModel model)
        {
            // Giới hạn tối đa 5 lần thử mỗi IP trong 15 phút
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"admin_invite_attempts_{ipAddress}";
            int attempts = _cache.Get<int>(cacheKey);

            if (attempts >= 5)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã thử kích hoạt mã mời quá 5 lần. Vui lòng thử lại sau 15 phút.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message, user) = await _authService.RegisterAdminByInviteAsync(
                model.FullName,
                model.Email,
                model.Password,
                model.InviteCode
            );

            if (!success || user == null)
            {
                _cache.Set(cacheKey, attempts + 1, TimeSpan.FromMinutes(15));
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            // Thành công -> xóa đếm giới hạn IP và đăng nhập luôn
            _cache.Remove(cacheKey);
            await SignInUserAsync(user);

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        // POST: /Admin/Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }

        // POST: /Admin/Auth/SaveAvatar
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAvatar([FromBody] AvatarPayload payload)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized(new { success = false, message = "Phiên đăng nhập không hợp lệ." });
            }

            var (success, message) = await _authService.SaveAvatarAsync(userId, payload?.AvatarJson);
            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message });
        }

        private async Task SignInUserAsync(Models.Entities.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Phiên Admin hết hạn sau 30 phút không thao tác
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );
        }

        public class AvatarPayload
        {
            public string? AvatarJson { get; set; }
        }
    }
}