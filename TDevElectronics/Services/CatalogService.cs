using TDevElectronics.Data;

namespace TDevElectronics.Services
{
    public class CatalogService : ICatalogService
    {
        public IReadOnlyList<SampleCategory> GetCategories() => SampleData.Categories;
        public SampleCategory? FindCategory(string? slugOrName) => SampleData.FindCategory(slugOrName);
        public SampleProduct? FindProduct(string? skuOrKey) => SampleData.Find(skuOrKey);
        public IReadOnlyList<SampleProduct> GetByCategory(string slug) => SampleData.InCategory(slug);
        public IReadOnlyList<SampleProduct> Related(SampleProduct product, int take = 4) => SampleData.Related(product, take);
        public IReadOnlyList<string> GetBrands() => SampleData.Brands;

        public IReadOnlyList<SampleProduct> Search(CatalogQuery q)
        {
            IEnumerable<SampleProduct> r = SampleData.Products;
            var term = q.Term?.Trim();
            if (!string.IsNullOrEmpty(term))
                r = r.Where(p => $"{p.Name} {p.Brand} {SampleData.CategoryName(p.CategorySlug)} {p.Spec} {p.Type}"
                    .Contains(term, StringComparison.OrdinalIgnoreCase));
            if (q.Categories is { Count: > 0 })
            {
                var slugs = q.Categories.Select(c => SampleData.FindCategory(c)?.Slug).Where(s => s != null).ToHashSet();
                r = r.Where(p => slugs.Contains(p.CategorySlug));
            }
            if (q.Brands is { Count: > 0 })
            {
                var brands = q.Brands.ToHashSet(StringComparer.OrdinalIgnoreCase);
                r = r.Where(p => brands.Contains(p.Brand));
            }
            if (q.MinPrice.HasValue) r = r.Where(p => p.BasePrice >= q.MinPrice.Value);
            if (q.MaxPrice.HasValue) r = r.Where(p => p.BasePrice <= q.MaxPrice.Value);
            if (q.InStockOnly) r = r.Where(p => p.Stock is null or > 0);
            if (q.BulkOnly) r = r.Where(p => p.HasBulk);
            r = q.Sort switch
            {
                "price-asc" => r.OrderBy(p => p.BasePrice),
                "price-desc" => r.OrderByDescending(p => p.BasePrice),
                "rating" => r.OrderByDescending(p => p.RatingValue).ThenByDescending(p => p.Reviews),
                _ => r
            };
            return r.ToList();
        }
    }
}
