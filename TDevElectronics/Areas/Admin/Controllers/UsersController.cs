using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDevElectronics.Data;
using TDevElectronics.Helpers;
using TDevElectronics.Models.Entities;
using TDevElectronics.Models.Enums;

namespace TDevElectronics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Users
        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? status, string? role, int page = 1)
        {
            const int pageSize = 15;
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
            }

            if (status == "active")
            {
                query = query.Where(u => u.IsActive);
            }
            else if (status == "inactive")
            {
                query = query.Where(u => !u.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(role) && UserRoles.IsValid(role))
            {
                query = query.Where(u => u.Role == role);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Status = status;
            ViewBag.Role = role;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(users);
        }

        // POST: /Admin/Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string role, bool isActive)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            if (!UserRoles.IsValid(role))
            {
                TempData["Error"] = "Vai trò không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            int currentAdminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (user.Id == currentAdminId && (!isActive || role != UserRoles.Admin))
            {
                TempData["Error"] = "Bạn không thể tự hạ quyền hoặc vô hiệu hóa chính mình.";
                return RedirectToAction(nameof(Index));
            }

            // Phải còn ít nhất 1 Admin đang hoạt động
            if (user.Role == UserRoles.Admin && user.IsActive && (role != UserRoles.Admin || !isActive))
            {
                int activeAdminCount = await _context.Users.CountAsync(u => u.Role == UserRoles.Admin && u.IsActive);
                if (activeAdminCount <= 1)
                {
                    TempData["Error"] = "Hệ thống phải có ít nhất 1 quản trị viên đang hoạt động.";
                    return RedirectToAction(nameof(Index));
                }
            }

            user.Role = role;
            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã cập nhật tài khoản {user.Email}.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Users/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            int currentAdminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (user.Id == currentAdminId)
            {
                TempData["Error"] = "Không thể tự xóa tài khoản của chính bạn.";
                return RedirectToAction(nameof(Index));
            }

            if (user.Role == UserRoles.Admin && user.IsActive)
            {
                int activeAdminCount = await _context.Users.CountAsync(u => u.Role == UserRoles.Admin && u.IsActive);
                if (activeAdminCount <= 1)
                {
                    TempData["Error"] = "Không thể xóa quản trị viên cuối cùng của hệ thống.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Lưu ý: Khi C và D thêm bảng Orders và Quotes vào AppDbContext,
            // sẽ kiểm tra thêm nếu user đã có đơn hoặc báo giá thì chỉ chuyển IsActive = false thay vì xóa cứng.
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa tài khoản {user.Email}.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/Invites
        [HttpGet]
        public async Task<IActionResult> Invites()
        {
            // Tự động cập nhật trạng thái Expired cho các mã quá hạn
            var expiredInvites = await _context.InviteCodes
                .Where(i => i.Status == InviteStatus.Active && i.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (expiredInvites.Any())
            {
                foreach (var item in expiredInvites)
                {
                    item.Status = InviteStatus.Expired;
                }
                await _context.SaveChangesAsync();
            }

            var invites = await _context.InviteCodes
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return View(invites);
        }

        // POST: /Admin/Users/CreateInvite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvite(int expiryHours = 24, string? targetEmail = null)
        {
            // Chỉ chấp nhận mốc 24h, 48h, 7 ngày (168h), 30 ngày (720h)
            int[] allowedHours = { 24, 48, 168, 720 };
            if (!allowedHours.Contains(expiryHours))
            {
                expiryHours = 24;
            }

            string rawCode = GenerateInviteCode();
            string codeHash = PasswordHelper.ComputeSha256(rawCode);
            string lastFour = rawCode[^4..];

            var invite = new InviteCode
            {
                CodeHash = codeHash,
                LastFourChars = lastFour,
                TargetEmail = string.IsNullOrWhiteSpace(targetEmail) ? null : targetEmail.Trim().ToLower(),
                Status = InviteStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(expiryHours)
            };

            await _context.InviteCodes.AddAsync(invite);
            await _context.SaveChangesAsync();

            // Mã chỉ hiển thị duy nhất một lần sau khi tạo
            TempData["NewInviteCode"] = rawCode;
            TempData["NewInviteLink"] = Url.Action("Register", "Auth", new { area = "Admin", code = rawCode }, Request.Scheme);
            TempData["Success"] = "Đã tạo mã mời quản trị mới.";

            return RedirectToAction(nameof(Invites));
        }

        // POST: /Admin/Users/RevokeInvite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeInvite(int id)
        {
            var invite = await _context.InviteCodes.FindAsync(id);
            if (invite == null)
            {
                TempData["Error"] = "Không tìm thấy mã mời.";
                return RedirectToAction(nameof(Invites));
            }

            if (invite.Status != InviteStatus.Active)
            {
                TempData["Error"] = "Chỉ có thể thu hồi mã mời đang hoạt động.";
                return RedirectToAction(nameof(Invites));
            }

            invite.Status = InviteStatus.Revoked;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã thu hồi mã mời ****-{invite.LastFourChars}.";
            return RedirectToAction(nameof(Invites));
        }

        // POST: /Admin/Users/RemoveInvite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveInvite(int id)
        {
            var invite = await _context.InviteCodes.FindAsync(id);
            if (invite != null)
            {
                _context.InviteCodes.Remove(invite);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bản ghi mã mời.";
            }

            return RedirectToAction(nameof(Invites));
        }

        // Sinh mã dạng XXXX-XXXX-XXXX-XXXX-XXXX (20 ký tự ngẫu nhiên an toàn)
        private static string GenerateInviteCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Span<char> buffer = stackalloc char[24];
            int pos = 0;

            for (int group = 0; group < 5; group++)
            {
                if (group > 0)
                {
                    buffer[pos++] = '-';
                }

                for (int i = 0; i < 4; i++)
                {
                    buffer[pos++] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
                }
            }

            return new string(buffer);
        }
    }
}