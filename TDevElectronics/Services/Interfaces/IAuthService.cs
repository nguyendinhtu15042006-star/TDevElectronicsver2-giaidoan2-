using TDevElectronics.Models.Entities;

namespace TDevElectronics.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password);
        Task<(bool Success, string Message, User? User)> RegisterCustomerAsync(string fullName, string email, string password);
        Task<(bool Success, string Message, User? User)> RegisterAdminByInviteAsync(string fullName, string email, string password, string inviteCode);
        Task<(bool Success, string Message)> SaveAvatarAsync(int userId, string? avatarJson);
    }
}