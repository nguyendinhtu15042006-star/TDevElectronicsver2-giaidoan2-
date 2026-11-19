using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.ViewModels
{
    /// <summary>Form thêm / sửa sản phẩm ở Admin (Products/_ProductForm). Giai đoạn 2 ánh xạ sang entity Product.</summary>
    public class ProductFormViewModel
    {
        public string? OriginalSku { get; set; }

        [Required(ErrorMessage = "Nhập tên sản phẩm."), StringLength(150)]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập mã SKU."), StringLength(40)]
        [RegularExpression(@"^[A-Za-z0-9\-_]+$", ErrorMessage = "SKU chỉ gồm chữ, số, dấu - và _.")]
        public string Sku { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public string CategorySlug { get; set; } = string.Empty;

        [StringLength(60)]
        public string? Brand { get; set; }

        [Range(0, 1_000_000_000, ErrorMessage = "Giá phải từ 0 trở lên.")]
        [Display(Name = "Giá bán lẻ")]
        public decimal Price { get; set; }

        [Display(Name = "Đơn vị tiền")]
        public string Currency { get; set; } = "VND";

        [Range(0, 100000, ErrorMessage = "Tồn kho từ 0 đến 100000.")]
        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        [Display(Name = "About this device")]
        public string? About { get; set; }

        [Display(Name = "Overview highlights (mỗi dòng một ý)")]
        public string? Highlights { get; set; }

        [Display(Name = "Technical specifications (mỗi dòng Tên: giá trị)")]
        public string? Specs { get; set; }

        [Display(Name = "Delivery & warranty (mỗi dòng một ý)")]
        public string? Delivery { get; set; }

        public List<(string Slug, string Name)> Categories { get; set; } = new();
        public bool IsEdit => !string.IsNullOrEmpty(OriginalSku);
    }
}
