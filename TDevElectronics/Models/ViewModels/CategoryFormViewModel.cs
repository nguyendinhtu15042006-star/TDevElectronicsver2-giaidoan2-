using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.ViewModels
{
    /// <summary>Form thêm / sửa danh mục ở Admin. Số thứ tự hiển thị được dồn lại 1..N khi lưu (giai đoạn 2).</summary>
    public class CategoryFormViewModel
    {
        public string? OriginalSlug { get; set; }

        [Required(ErrorMessage = "Nhập tên danh mục."), StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập slug."), StringLength(80)]
        [RegularExpression(@"^[a-z0-9]+(-[a-z0-9]+)*$", ErrorMessage = "Slug chỉ gồm chữ thường, số và dấu gạch ngang.")]
        public string Slug { get; set; } = string.Empty;

        [Range(1, 99, ErrorMessage = "Thứ tự từ 1 đến 99.")]
        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;
        public bool IsEdit => !string.IsNullOrEmpty(OriginalSlug);
    }
}
