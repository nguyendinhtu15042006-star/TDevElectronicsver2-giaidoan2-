using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Danh mục sản phẩm. Giai đoạn 1: lớp dữ liệu (chưa có DbSet); dữ liệu thật đang ở Data/SampleData.cs.</summary>
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(80)] public string Name { get; set; } = string.Empty;
        [Required, MaxLength(80)] public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        [MaxLength(300)] public string? ImageUrl { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}
