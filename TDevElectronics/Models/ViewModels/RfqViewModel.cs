using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.ViewModels
{
    /// <summary>Form yêu cầu báo giá (Quotes/Rfq). Cần ít nhất một dòng sản phẩm có số lượng &gt; 0.</summary>
    public class RfqViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Nhập tên công ty."), StringLength(150)] public string Company { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nhập tên người liên hệ."), StringLength(100)] public string ContactName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nhập email doanh nghiệp."), EmailAddress(ErrorMessage = "Email không hợp lệ.")] public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nhập số điện thoại."), Phone(ErrorMessage = "Số điện thoại không hợp lệ.")] public string Phone { get; set; } = string.Empty;
        [StringLength(40)] public string? TaxId { get; set; }
        [Required(ErrorMessage = "Nhập tên dự án."), StringLength(150)] public string ProjectName { get; set; } = string.Empty;
        [DataType(DataType.Date)] public DateTime? TargetDeliveryDate { get; set; }
        [Range(0, 100_000_000_000, ErrorMessage = "Ngân sách không hợp lệ.")] public decimal? Budget { get; set; }
        [StringLength(300)] public string? DeliveryAddress { get; set; }
        [StringLength(1000)] public string? Notes { get; set; }
        public List<RfqLine> Lines { get; set; } = new();
        [Range(typeof(bool), "true", "true", ErrorMessage = "Hãy đồng ý để chúng tôi liên hệ.")] public bool AgreePrivacy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (!Lines.Any(l => l.Quantity > 0))
                yield return new ValidationResult("Chọn ít nhất một sản phẩm với số lượng lớn hơn 0.", new[] { nameof(Lines) });
            if (TargetDeliveryDate is { } d && d.Date < DateTime.Today)
                yield return new ValidationResult("Ngày giao dự kiến phải từ hôm nay trở đi.", new[] { nameof(TargetDeliveryDate) });
        }
    }

    public class RfqLine
    {
        [Required] public string Sku { get; set; } = string.Empty;
        [Range(0, 100000)] public int Quantity { get; set; }
    }
}
