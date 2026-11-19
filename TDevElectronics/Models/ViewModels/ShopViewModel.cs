using TDevElectronics.Data;

namespace TDevElectronics.Models.ViewModels
{
    public sealed record FilterOption(string Value, string Label, int Count, bool Selected);

    public sealed record PriceBand(string Title, string Note, IReadOnlyList<SampleProduct> Items);

    /// <summary>Trang /Shop: danh sách, bộ lọc, sắp xếp, phân trang.</summary>
    public class ShopViewModel
    {
        public const int PageSize = 12;

        public IReadOnlyList<SampleProduct> Products { get; set; } = Array.Empty<SampleProduct>();
        public int Total { get; set; }
        public int Page { get; set; } = 1;
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(Total / (double)PageSize));
        public int From => Total == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int To => Math.Min(Total, Page * PageSize);

        public string? Query { get; set; }
        public string Sort { get; set; } = "recommended";
        public List<string> Categories { get; set; } = new();
        public List<string> Brands { get; set; } = new();
        public bool InStockOnly { get; set; }
        public bool BulkOnly { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }

        public IReadOnlyList<FilterOption> CategoryOptions { get; set; } = Array.Empty<FilterOption>();
        public IReadOnlyList<FilterOption> BrandOptions { get; set; } = Array.Empty<FilterOption>();

        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(Query) || Categories.Count > 0 || Brands.Count > 0 ||
            InStockOnly || BulkOnly || PriceMin.HasValue || PriceMax.HasValue;

        public static readonly (string Value, string Label)[] SortOptions =
        {
            ("recommended", "Recommended"),
            ("price-asc", "Price: Low to High"),
            ("price-desc", "Price: High to Low"),
            ("rating", "Top rated"),
        };
    }

    /// <summary>Trang /Shop/Category/{slug}: ảnh đầu trang, nhóm theo mức giá, bảng lọc phía trình duyệt.</summary>
    public class CategoryViewModel
    {
        public required SampleCategory Category { get; init; }
        public required SampleProduct Hero { get; init; }
        public IReadOnlyList<SampleCategory> AllCategories { get; init; } = Array.Empty<SampleCategory>();
        public IReadOnlyList<SampleProduct> Products { get; init; } = Array.Empty<SampleProduct>();
        public IReadOnlyList<PriceBand> Bands { get; init; } = Array.Empty<PriceBand>();
        public IReadOnlyList<(string Brand, int Count)> Brands { get; init; } = Array.Empty<(string, int)>();
        public IReadOnlyList<string> Types { get; init; } = Array.Empty<string>();
    }
}
