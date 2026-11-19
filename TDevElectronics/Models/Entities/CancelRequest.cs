using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Yêu cầu hủy đơn + lý do (Phần việc D). Do khách gửi, admin duyệt hoặc từ chối kèm lý do.</summary>
    public class CancelRequest
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [Required, MaxLength(500)] public string Reason { get; set; } = string.Empty;
        public bool RequestedByAdmin { get; set; }
        public bool? Approved { get; set; }              // null = đang chờ duyệt
        [MaxLength(500)] public string? RejectReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
    }
}
