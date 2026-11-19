using System.Text.Json;
using TDevElectronics.Data;

namespace TDevElectronics.Models.ViewModels
{
    public sealed record CartLine(string Key, string Sku, string Name, string Brand, int Quantity, decimal UnitPrice)
    {
        public decimal LineTotal => UnitPrice * Quantity;
    }

    /// <summary>
    /// Giỏ hàng phía server (lưu trong Session, dữ liệu giá lấy từ SampleData).
    /// Quy tắc tính tiền khớp với tot() trong app.js: giá theo bậc số lượng, phí ship 9 USD
    /// (miễn phí từ 500 USD), thuế VAT 8%.
    /// </summary>
    public class CartViewModel
    {
        public const string SessionKey = "tdev.cart";
        public const decimal FreeShippingFrom = 500m;
        public const decimal ShippingFlat = 9m;
        public const decimal TaxPercent = 8m;
        public const int MaxQuantity = 999;

        public IReadOnlyList<CartLine> Lines { get; init; } = Array.Empty<CartLine>();

        public int ItemCount => Lines.Sum(l => l.Quantity);
        public decimal Subtotal => Lines.Sum(l => l.LineTotal);
        public decimal Shipping => Subtotal == 0 || Subtotal >= FreeShippingFrom ? 0m : ShippingFlat;
        public decimal Tax => Math.Round(Subtotal * TaxPercent, MidpointRounding.AwayFromZero) / 100m;
        public decimal Total => Subtotal + Shipping + Tax;
        public decimal AmountToFreeShipping => Math.Max(0m, FreeShippingFrom - Subtotal);
        public bool IsEmpty => Lines.Count == 0;

        /// <summary>Đơn giá theo bậc số lượng (bậc lớn nhất có MinQty không vượt quá số lượng).</summary>
        public static decimal UnitPrice(SampleProduct p, int qty)
        {
            var tier = p.Tiers.Where(t => qty >= t.MinQty).OrderByDescending(t => t.MinQty).FirstOrDefault()
                       ?? p.Tiers.OrderBy(t => t.MinQty).First();
            return tier.Unit;
        }

        /// <summary>Dựng giỏ từ danh sách (key hoặc sku, số lượng); bỏ qua sản phẩm không tồn tại.</summary>
        public static CartViewModel From(IEnumerable<KeyValuePair<string, int>> items)
        {
            var lines = new List<CartLine>();
            foreach (var (k, q) in items)
            {
                var p = SampleData.Find(k);
                var qty = Math.Clamp(q, 0, MaxQuantity);
                if (p is null || qty == 0) continue;
                lines.Add(new CartLine(p.Key, p.Sku, p.Name, p.Brand, qty, UnitPrice(p, qty)));
            }
            return new CartViewModel { Lines = lines };
        }

        /// <summary>Đọc giỏ (key → số lượng) từ Session.</summary>
        public static Dictionary<string, int> Load(ISession session)
        {
            var raw = session.GetString(SessionKey);
            if (string.IsNullOrEmpty(raw)) return new Dictionary<string, int>();
            try { return JsonSerializer.Deserialize<Dictionary<string, int>>(raw) ?? new(); }
            catch (JsonException) { return new Dictionary<string, int>(); }
        }

        public static void Save(ISession session, Dictionary<string, int> items)
        {
            var clean = items.Where(kv => kv.Value > 0 && SampleData.Find(kv.Key) is not null)
                .ToDictionary(kv => SampleData.Find(kv.Key)!.Key, kv => Math.Clamp(kv.Value, 1, MaxQuantity));
            session.SetString(SessionKey, JsonSerializer.Serialize(clean));
        }

        /// <summary>Dữ liệu tóm tắt trả về cho JavaScript sau mỗi thao tác Add / Update / Remove.</summary>
        public object ToJson() => new
        {
            count = ItemCount,
            subtotal = Subtotal,
            shipping = Shipping,
            tax = Tax,
            total = Total,
            toFreeShipping = AmountToFreeShipping,
            lines = Lines.Select(l => new { key = l.Key, sku = l.Sku, name = l.Name, qty = l.Quantity, unit = l.UnitPrice, line = l.LineTotal })
        };
    }
}
