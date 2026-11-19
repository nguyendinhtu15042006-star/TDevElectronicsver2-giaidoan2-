using System.ComponentModel.DataAnnotations;
using TDevElectronics.Models.Enums;

namespace TDevElectronics.Models.Entities
{
    /// <summary>Yêu cầu báo giá (RFQ) của khách doanh nghiệp (Phần việc D).</summary>
    public class Quote
    {
        public int Id { get; set; }
        [Required, MaxLength(20)] public string Code { get; set; } = string.Empty;   // Q-2084
        public int? UserId { get; set; }
        public User? User { get; set; }
        [Required, MaxLength(150)] public string Company { get; set; } = string.Empty;
        [Required, MaxLength(100)] public string ContactName { get; set; } = string.Empty;
        [Required, MaxLength(150)] public string Email { get; set; } = string.Empty;
        [MaxLength(30)] public string? Phone { get; set; }
        [MaxLength(40)] public string? TaxId { get; set; }
        [Required, MaxLength(150)] public string ProjectName { get; set; } = string.Empty;
        public DateTime? TargetDeliveryDate { get; set; }
        public decimal? Budget { get; set; }
        [MaxLength(300)] public string? DeliveryAddress { get; set; }
        [MaxLength(1000)] public string? Notes { get; set; }
        public QuoteStatus Status { get; set; } = QuoteStatus.UnderReview;
        public decimal AmountBeforeTax { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<QuoteItem> Items { get; set; } = new();
    }
}
