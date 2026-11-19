using TDevElectronics.Data;

namespace TDevElectronics.Models.ViewModels
{
    public sealed record TierRow(string Range, string Price, string Savings, bool Highlight);

    public sealed record GalleryItem(string Label, string Css);

    public class ProductDetailViewModel
    {
        public required SampleProduct Product { get; init; }
        public required SampleCategory Category { get; init; }
        public IReadOnlyList<SampleProduct> Related { get; init; } = Array.Empty<SampleProduct>();
        public IReadOnlyList<TierRow> TierRows { get; init; } = Array.Empty<TierRow>();
        public IReadOnlyList<GalleryItem> Gallery { get; init; } = Array.Empty<GalleryItem>();
        public string Eta { get; init; } = "";
        public string Stars { get; init; } = "★★★★★";

        /// <summary>Danh sách mốc giá dạng JSON cho shop.js: [[minQty, unit], ...] (tăng dần).</summary>
        public string TiersJson =>
            "[" + string.Join(",", Product.Tiers.OrderBy(t => t.MinQty)
                .Select(t => FormattableString.Invariant($"[{t.MinQty},{t.Unit}]"))) + "]";
    }
}
