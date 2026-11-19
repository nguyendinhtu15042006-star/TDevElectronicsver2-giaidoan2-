using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Ảnh phụ của sản phẩm, tối đa 3 ảnh (MaxPerProduct).</summary>
    public class ProductImage
    {
        public const int MaxPerProduct = 3;

        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required, MaxLength(300)] public string Url { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
