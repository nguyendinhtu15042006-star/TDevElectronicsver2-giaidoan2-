using TDevElectronics.Data;

namespace TDevElectronics.Services
{
    /// <summary>Bộ lọc danh sách sản phẩm (cùng ý nghĩa tham số của /Shop).</summary>
    public sealed record CatalogQuery(string? Term = null, IReadOnlyCollection<string>? Categories = null,
        IReadOnlyCollection<string>? Brands = null, decimal? MinPrice = null, decimal? MaxPrice = null,
        bool InStockOnly = false, bool BulkOnly = false, string? Sort = null);

    /// <summary>Sản phẩm, danh mục, lọc, tìm kiếm. Giai đoạn 1 đọc SampleData; giai đoạn 2 đọc AppDbContext.</summary>
    public interface ICatalogService
    {
        IReadOnlyList<SampleCategory> GetCategories();
        SampleCategory? FindCategory(string? slugOrName);
        SampleProduct? FindProduct(string? skuOrKey);
        IReadOnlyList<SampleProduct> GetByCategory(string slug);
        IReadOnlyList<SampleProduct> Search(CatalogQuery query);
        IReadOnlyList<SampleProduct> Related(SampleProduct product, int take = 4);
        IReadOnlyList<string> GetBrands();
    }
}
