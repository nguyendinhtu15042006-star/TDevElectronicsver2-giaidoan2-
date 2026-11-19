using System.ComponentModel.DataAnnotations;

namespace TDevElectronics.Models.Entities
{
    /// <summary>
    /// Địa chỉ giao hàng của một đơn (Phần việc C). Giai đoạn 1 chỉ là lớp dữ liệu dùng để kiểm tra form;
    /// giai đoạn 2 sẽ thêm vào AppDbContext và nối với Order.
    /// </summary>
    public class Address
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? Company { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>Mã quốc gia ISO 3166-1 alpha-2, ví dụ VN, US.</summary>
        [Required, StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; } = "VN";

        [Required, MaxLength(200)]
        public string Line1 { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Line2 { get; set; }

        /// <summary>Phường / xã (Việt Nam).</summary>
        [MaxLength(100)]
        public string? Ward { get; set; }

        /// <summary>Quận / huyện (nếu có).</summary>
        [MaxLength(100)]
        public string? District { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>Tỉnh / thành, bang hoặc vùng.</summary>
        [MaxLength(100)]
        public string? Region { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        public bool IsDefault { get; set; }

        /// <summary>Địa chỉ gọn trên một dòng để hiển thị ở bước xác nhận.</summary>
        public string OneLine()
        {
            // Việt Nam không có ô "thành phố" riêng (City được gán bằng tỉnh/thành), nên bỏ City nếu trùng Region.
            var city = string.Equals(City?.Trim(), Region?.Trim(), StringComparison.OrdinalIgnoreCase) ? null : City;
            var parts = new[] { Line1, Line2, Ward, District, city, Region, PostalCode }
                .Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim());
            return string.Join(", ", parts);
        }
    }
}
