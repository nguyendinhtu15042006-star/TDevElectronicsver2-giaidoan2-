using System.ComponentModel.DataAnnotations;
using TDevElectronics.Models.Enums;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Đơn hàng (Phần việc D).
    /// Giai đoạn 1: lớp dữ liệu, CHƯA có DbSet trong AppDbContext. Giai đoạn 2 thêm DbSet + Add-Migration.
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        [Required, MaxLength(20)] public string Code { get; set; } = string.Empty;   // TD-1048
        public int? UserId { get; set; }
        public User? User { get; set; }
        [MaxLength(100)] public string CustomerName { get; set; } = string.Empty;
        [MaxLength(150)] public string CustomerEmail { get; set; } = string.Empty;
        [MaxLength(300)] public string ShippingAddress { get; set; } = string.Empty;
        public PaymentMethod Payment { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }                 // VAT 8%
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();
        public List<CancelRequest> CancelRequests { get; set; } = new();
    }
}
