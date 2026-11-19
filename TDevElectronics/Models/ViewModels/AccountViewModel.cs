namespace TDevElectronics.Models.ViewModels
{
    /// <summary>
    /// Dữ liệu cho trang tài khoản khách hàng (Account/Index). Giai đoạn 1 lấy từ khách demo,
    /// giai đoạn 2 lấy từ người dùng đang đăng nhập (User) trong CSDL.
    /// </summary>
    public class AccountViewModel
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Company { get; init; } = string.Empty;
        public string JobTitle { get; init; } = string.Empty;

        /// <summary>Số đơn của khách (đơn mẫu giai đoạn 1).</summary>
        public int OrderCount { get; init; }

        /// <summary>Số đơn đang chờ xử lý hoặc đang xử lý, có thể gửi yêu cầu hủy.</summary>
        public int CancellableCount { get; init; }

        public string FirstName => FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? FullName;

        public string Initials => string.Concat(
            FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => char.ToUpperInvariant(w[0])));
    }
}
