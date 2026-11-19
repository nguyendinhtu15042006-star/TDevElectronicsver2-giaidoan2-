using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.ViewModels
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Email")] public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nhập họ tên."), StringLength(100)] public string FullName { get; set; } = string.Empty;
        [Required, RegularExpression("^(Customer|Admin)$", ErrorMessage = "Vai trò không hợp lệ.")] public string Role { get; set; } = "Customer";
        public bool IsActive { get; set; } = true;
    }
}
