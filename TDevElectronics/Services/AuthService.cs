using Microsoft.EntityFrameworkCore;
using TDevElectronics.Data;
using TDevElectronics.Helpers;
using TDevElectronics.Models.Entities;
using TDevElectronics.Models.Enums;
using TDevElectronics.Services.Interfaces;

namespace TDevElectronics.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password)
        {
            var normalizedEmail = email.Trim().ToLower();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            // Thông báo lỗi chung chung để tránh dò email
            if (user == null)
            {
                return (false, "Email hoặc mật khẩu không chính xác.", null);
            }

            if (!user.IsActive)
            {
                return (false, "Tài khoản của bạn đã bị vô hiệu hóa.", null);
            }

            // Kiểm tra khóa tạm thời (8 lần sai / 15 phút)
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                var remainingMinutes = Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
                return (false, $"Tài khoản tạm khóa do đăng nhập sai nhiều lần. Vui lòng thử lại sau {remainingMinutes} phút.", null);
            }

            bool isValid = PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 8)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.FailedLoginAttempts = 0;
                    await _context.SaveChangesAsync();
                    return (false, "Bạn đã nhập sai quá 8 lần. Tài khoản bị khóa trong 15 phút.", null);
                }

                await _context.SaveChangesAsync();
                return (false, "Email hoặc mật khẩu không chính xác.", null);
            }

            // Đăng nhập thành công -> reset đếm lỗi
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return (true, "Đăng nhập thành công.", user);
        }

        public async Task<(bool Success, string Message, User? User)> RegisterCustomerAsync(string fullName, string email, string password)
        {
            var normalizedEmail = email.Trim().ToLower();
            var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
            if (exists)
            {
                return (false, "Email này đã được sử dụng.", null);
            }

            var (hash, salt) = PasswordHelper.HashPassword(password);
            var newUser = new User
            {
                FullName = fullName.Trim(),
                Email = normalizedEmail,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = UserRoles.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return (true, "Đăng ký thành công.", newUser);
        }

        public async Task<(bool Success, string Message, User? User)> RegisterAdminByInviteAsync(string fullName, string email, string password, string inviteCode)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            {
                return (false, "Mật khẩu quản trị viên phải có tối thiểu 12 ký tự.", null);
            }

            var normalizedEmail = email.Trim().ToLower();
            var cleanCode = inviteCode.Trim().ToUpperInvariant();
            var codeHash = PasswordHelper.ComputeSha256(cleanCode);

            var invite = await _context.InviteCodes.FirstOrDefaultAsync(i => i.CodeHash == codeHash);
            if (invite == null)
            {
                return (false, "Mã mời không hợp lệ.", null);
            }

            if (invite.Status == InviteStatus.Revoked)
            {
                return (false, "Mã mời này đã bị thu hồi.", null);
            }

            if (invite.Status == InviteStatus.Used || invite.UsedAt.HasValue)
            {
                return (false, "Mã mời này đã được sử dụng.", null);
            }

            if (invite.Status == InviteStatus.Expired || invite.ExpiresAt < DateTime.UtcNow)
            {
                if (invite.Status == InviteStatus.Active)
                {
                    invite.Status = InviteStatus.Expired;
                    await _context.SaveChangesAsync();
                }
                return (false, "Mã mời đã hết hạn.", null);
            }

            if (!string.IsNullOrWhiteSpace(invite.TargetEmail) && invite.TargetEmail.ToLower() != normalizedEmail)
            {
                return (false, "Mã mời này được cấp riêng cho một địa chỉ email khác.", null);
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
            if (emailExists)
            {
                return (false, "Email này đã tồn tại trong hệ thống.", null);
            }

            var (hash, salt) = PasswordHelper.HashPassword(password);
            var adminUser = new User
            {
                FullName = fullName.Trim(),
                Email = normalizedEmail,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = UserRoles.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync();

            // Cập nhật trạng thái mã mời đã dùng
            invite.Status = InviteStatus.Used;
            invite.UsedAt = DateTime.UtcNow;
            invite.UsedByUserId = adminUser.Id;
            await _context.SaveChangesAsync();

            return (true, "Kích hoạt tài khoản quản trị thành công.", adminUser);
        }

        public async Task<(bool Success, string Message)> SaveAvatarAsync(int userId, string? avatarJson)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy người dùng.");
            }

            // Giới hạn kích thước dữ liệu ảnh base64 (~2MB text)
            if (!string.IsNullOrEmpty(avatarJson) && avatarJson.Length > 2_500_000)
            {
                return (false, "Dung lượng ảnh đại diện vượt quá giới hạn cho phép.");
            }

            user.AvatarJson = string.IsNullOrWhiteSpace(avatarJson) ? null : avatarJson;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (true, "Đã cập nhật ảnh đại diện.");
        }
    }
}