using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Một dòng sản phẩm trong đơn (Phần việc D). Lưu tên và đơn giá tại thời điểm đặt.</summary>
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [Required, MaxLength(40)] public string Sku { get; set; } = string.Empty;
        [Required, MaxLength(150)] public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
