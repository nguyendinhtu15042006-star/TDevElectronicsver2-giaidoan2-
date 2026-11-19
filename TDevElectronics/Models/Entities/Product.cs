using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Sản phẩm. Giai đoạn 1: lớp dữ liệu (chưa có DbSet). Giá theo số lượng nằm ở PriceTier, ảnh phụ ở ProductImage.</summary>
    public class Product
    {
        public int Id { get; set; }
        [Required, MaxLength(40)] public string Sku { get; set; } = string.Empty;
        [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
        [MaxLength(60)] public string? Brand { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [MaxLength(200)] public string? Spec { get; set; }
        public string? About { get; set; }
        public string? Highlights { get; set; }      // mỗi dòng một ý
        public string? Specs { get; set; }           // mỗi dòng "Tên: giá trị"
        public string? Delivery { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        [MaxLength(300)] public string? MainImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ProductImage> Images { get; set; } = new();
        public List<PriceTier> PriceTiers { get; set; } = new();
    }
}
