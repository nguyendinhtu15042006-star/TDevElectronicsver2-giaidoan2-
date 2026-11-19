using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.ViewModels
{
    /// <summary>Khách gửi yêu cầu hủy đơn. Chọn lý do "Khác" thì phải ghi chi tiết tối thiểu 5 ký tự.</summary>
    public class CancelRequestViewModel : IValidatableObject
    {
        [Required] public string OrderId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Hãy chọn lý do hủy.")] public string Reason { get; set; } = string.Empty;
        [StringLength(500)] public string? Detail { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (Reason == "Khác" && (Detail ?? "").Trim().Length < 5)
                yield return new ValidationResult("Hãy ghi rõ lý do (ít nhất 5 ký tự).", new[] { nameof(Detail) });
        }
    }
}
