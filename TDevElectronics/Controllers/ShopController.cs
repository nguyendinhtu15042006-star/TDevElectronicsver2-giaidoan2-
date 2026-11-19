using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Controllers
{
    public class ShopController : Controller
    {
        // /Shop?q=&cat=phones&cat=laptops&brand=Apple&min=0&max=1500&stock=true&bulk=true&sort=price-asc&page=2
        public IActionResult Index(string? q, string[]? cat, string[]? brand, string? sort,
                                   decimal? min, decimal? max, bool stock = false, bool bulk = false, int page = 1)
        {
            // chấp nhận cả slug (phones) lẫn tên (Office Furniture) cho liên kết cũ ?cat=Name
            var catSlugs = (cat ?? Array.Empty<string>())
                .Select(c => SampleData.FindCategory(c)?.Slug)
                .Where(s => s != null).Select(s => s!).Distinct().ToList();
            var brandSet = (brand ?? Array.Empty<string>()).Where(b => !string.IsNullOrWhiteSpace(b)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var term = q?.Trim();

            // Các lượt đếm bên bộ lọc tính trên tập đã lọc bởi tìm kiếm, giá, kho, giá sỉ (chưa lọc danh mục/thương hiệu).
            IEnumerable<SampleProduct> baseSet = SampleData.Products;
            if (!string.IsNullOrEmpty(term))
                baseSet = baseSet.Where(p => $"{p.Name} {p.Brand} {SampleData.CategoryName(p.CategorySlug)} {p.Spec} {p.Type}"
                    .Contains(term, StringComparison.OrdinalIgnoreCase));
            if (min.HasValue) baseSet = baseSet.Where(p => p.BasePrice >= min.Value);
            if (max.HasValue) baseSet = baseSet.Where(p => p.BasePrice <= max.Value);
            if (stock) baseSet = baseSet.Where(p => p.Stock is null or > 0);
            if (bulk) baseSet = baseSet.Where(p => p.HasBulk);
            var baseList = baseSet.ToList();

            IEnumerable<SampleProduct> result = baseList;
            if (catSlugs.Count > 0) result = result.Where(p => catSlugs.Contains(p.CategorySlug));
            if (brandSet.Count > 0) result = result.Where(p => brandSet.Contains(p.Brand));

            sort = ShopViewModel.SortOptions.Any(s => s.Value == sort) ? sort! : "recommended";
            result = sort switch
            {
                "price-asc" => result.OrderBy(p => p.BasePrice),
                "price-desc" => result.OrderByDescending(p => p.BasePrice),
                "rating" => result.OrderByDescending(p => p.RatingValue).ThenByDescending(p => p.Reviews),
                _ => result
            };
            var all = result.ToList();

            var vm = new ShopViewModel
            {
                Query = term, Sort = sort, Categories = catSlugs, Brands = brandSet.ToList(),
                InStockOnly = stock, BulkOnly = bulk, PriceMin = min, PriceMax = max, Total = all.Count,
                CategoryOptions = SampleData.Categories
                    .Select(c => new FilterOption(c.Slug, c.Name, baseList.Count(p => p.CategorySlug == c.Slug), catSlugs.Contains(c.Slug))).ToList(),
                BrandOptions = SampleData.Brands
                    .Select(b => new FilterOption(b, b, baseList.Count(p => p.Brand == b), brandSet.Contains(b)))
                    .Where(o => o.Count > 0 || o.Selected).ToList(),
            };
            vm.Page = Math.Clamp(page, 1, vm.TotalPages);
            vm.Products = all.Skip((vm.Page - 1) * ShopViewModel.PageSize).Take(ShopViewModel.PageSize).ToList();
            return View(vm);
        }

        // /Shop/Category/phones (route "category" khai báo trong Program.cs)
        public IActionResult Category(string slug)
        {
            var cat = SampleData.FindCategory(slug);
            if (cat == null) return NotFound();

            var items = SampleData.InCategory(cat.Slug);
            var hero = SampleData.Find(cat.HeroKey) ?? items.First();
            string M(decimal v) => v.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-US"));

            var bands = new List<PriceBand>
            {
                new($"Under {M(cat.Low)}", cat.BandNotes[0], items.Where(p => p.BasePrice < cat.Low).ToList()),
                new($"{M(cat.Low)} to {M(cat.High - 1)}", cat.BandNotes[1], items.Where(p => p.BasePrice >= cat.Low && p.BasePrice < cat.High).ToList()),
                new($"{M(cat.High)} and up", cat.BandNotes[2], items.Where(p => p.BasePrice >= cat.High).ToList()),
            }.Where(b => b.Items.Count > 0).ToList();

            return View(new CategoryViewModel
            {
                Category = cat, Hero = hero, AllCategories = SampleData.Categories, Products = items, Bands = bands,
                Brands = items.GroupBy(p => p.Brand).Select(g => (g.Key, g.Count())).OrderByDescending(x => x.Item2).ThenBy(x => x.Key).ToList(),
                Types = items.Select(p => p.Type).Distinct().OrderBy(t => t).ToList(),
            });
        }
    }
}
