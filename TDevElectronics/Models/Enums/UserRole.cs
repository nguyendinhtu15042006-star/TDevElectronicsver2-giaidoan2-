namespace TDevElectronics.Models.Enums
{
    /// <summary>Vai trò người dùng. User.Role lưu chuỗi, dùng các hằng này để tránh gõ sai.</summary>
    public enum UserRole { Customer, Admin }

    public static class UserRoles
    {
        public const string Customer = nameof(UserRole.Customer);
        public const string Admin = nameof(UserRole.Admin);
        public static bool IsValid(string? r) => r is Customer or Admin;
    }
}
