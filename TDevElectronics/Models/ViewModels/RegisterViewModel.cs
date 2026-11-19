using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.ViewModels
{
    // Đăng ký khách hàng (Account/Register)
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter your first name.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your last name.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        // Họ tên đầy đủ ghép từ 2 ô First/Last, dùng cho cột User.FullName (get-only nên không bind từ form)
        public string FullName => $"{FirstName.Trim()} {LastName.Trim()}".Trim();

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your phone number.")]
        [StringLength(30)]
        [RegularExpression(@"^(?=(?:\D*\d){7,})[0-9+()\-.\s]+$", ErrorMessage = "Enter a valid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
