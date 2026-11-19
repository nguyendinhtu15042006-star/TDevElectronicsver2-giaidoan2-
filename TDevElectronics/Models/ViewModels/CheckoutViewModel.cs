using System.ComponentModel.DataAnnotations;
using TDevElectronics.Helpers;
using TDevElectronics.Models.Enums;

namespace TDevElectronics.Models.ViewModels
{
    public sealed record PaymentOption(PaymentMethod Method, string Key, string Name, string Status, decimal Fee);

    /// <summary>Dữ liệu cho trang Checkout 4 bước (địa chỉ, giao hàng, thanh toán, xác nhận).</summary>
    public class CheckoutViewModel
    {
        public required CartViewModel Cart { get; init; }
        public IReadOnlyList<Country> Countries { get; init; } = CountryData.All;
        public IReadOnlyList<PaymentOption> Payments { get; init; } =
            Enum.GetValues<PaymentMethod>()
                .Select(m => new PaymentOption(m, m.Key(), m.DisplayName(), m.Status(), m.Fee()))
                .ToList();
        public string DefaultCountry { get; init; } = CountryData.DefaultCountry;
    }

    public class PlaceOrderItem
    {
        public string Key { get; set; } = string.Empty;
        public int Qty { get; set; }
    }

    /// <summary>Dữ liệu trình duyệt gửi lên khi đặt hàng (PlaceOrder).</summary>
    public class PlaceOrderRequest
    {
        /// <summary>Mã đơn hiển thị cho khách, ví dụ TD-1049 (do trình duyệt cấp, server kiểm tra định dạng).</summary>
        [MaxLength(20)]
        public string? Ref { get; set; }

        [Required(ErrorMessage = "Enter your full name."), StringLength(100, MinimumLength = 2, ErrorMessage = "Enter your full name.")]
        public string FullName { get; set; } = string.Empty;
        [StringLength(120)]
        public string? Company { get; set; }
        [Required(ErrorMessage = "Enter a valid email address."), EmailAddress(ErrorMessage = "Enter a valid email address."), StringLength(150)]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Enter a valid phone number."), StringLength(30)]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Choose a country."), StringLength(2, MinimumLength = 2, ErrorMessage = "Choose a country.")]
        public string CountryCode { get; set; } = CountryData.DefaultCountry;
        [Required(ErrorMessage = "Enter your street address."), StringLength(200)]
        public string Line1 { get; set; } = string.Empty;
        [StringLength(200)]
        public string? Line2 { get; set; }
        [StringLength(100)]
        public string? Ward { get; set; }
        [StringLength(100)]
        public string? District { get; set; }
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Region { get; set; }
        [StringLength(20)]
        public string? PostalCode { get; set; }

        /// <summary>Khóa phương thức thanh toán: card, paypal, apple, google, qr, bank, net30, bnpl, cod.</summary>
        [Required(ErrorMessage = "Choose a payment method.")]
        public string Payment { get; set; } = "card";

        public List<PlaceOrderItem> Items { get; set; } = new();
    }

    /// <summary>Đơn vừa đặt, lưu trong Session để trang Success hiển thị (giai đoạn 2 chuyển sang bảng Order).</summary>
    public class OrderSummary
    {
        public string Ref { get; set; } = string.Empty;
        public DateTime PlacedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ShipTo { get; set; } = string.Empty;
        public string PaymentKey { get; set; } = "card";
        public string PaymentName { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string Eta { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Fee { get; set; }
        public decimal Total { get; set; }
        public List<OrderSummaryLine> Lines { get; set; } = new();
    }

    public class OrderSummaryLine
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Unit { get; set; }
        public decimal Line { get; set; }
    }
}
