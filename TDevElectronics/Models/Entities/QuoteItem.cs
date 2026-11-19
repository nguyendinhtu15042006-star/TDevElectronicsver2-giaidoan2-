using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Một dòng sản phẩm trong báo giá (Phần việc D).</summary>
    public class QuoteItem
    {
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public Quote? Quote { get; set; }
        [Required, MaxLength(40)] public string Sku { get; set; } = string.Empty;
        [Required, MaxLength(150)] public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal QuotedUnitPrice { get; set; }
    }
}
