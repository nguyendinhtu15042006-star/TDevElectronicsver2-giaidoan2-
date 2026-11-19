using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.ViewModels
{
    // Dùng chung cho đăng nhập khách hàng (Account/Login) và quản trị (Admin/Auth/Login)
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
