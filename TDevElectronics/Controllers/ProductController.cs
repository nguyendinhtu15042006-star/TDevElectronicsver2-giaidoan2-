using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Controllers
{
    public class ProductController : Controller
    {
        // /Product/Detail/{sku} (route "product" khai báo trong Program.cs); cũng nhận key của template (vd. "laptop")
        public IActionResult Detail(string sku)
        {
            var p = SampleData.Find(sku);
            if (p == null) return NotFound();

            var cat = SampleData.Categories.First(c => c.Slug == p.CategorySlug);
            var asc = p.Tiers.OrderBy(t => t.MinQty).ToList();
            var baseUnit = p.BasePrice;
            var rows = asc.Select((t, i) =>
            {
                var next = i + 1 < asc.Count ? asc[i + 1] : null;
                var range = next == null ? $"{t.MinQty}+ units"
                          : next.MinQty - 1 > t.MinQty ? $"{t.MinQty}–{next.MinQty - 1} units" : $"{t.MinQty} unit";
                var save = t.Unit < baseUnit ? $"{SampleData.Money(baseUnit - t.Unit).Replace(".00", "")} / unit" : "Retail";
                return new TierRow(range, SampleData.Money(t.Unit), save, i == 0);
            }).ToList();

            var rating = (int)Math.Round(p.RatingValue);
            return View(new ProductDetailViewModel
            {
                Product = p, Category = cat, Related = SampleData.Related(p), TierRows = rows,
                Gallery = p.Views.Select(v => new GalleryItem(v.Label, v.Css(p.Aspect))).ToList(),
                Eta = Eta(), Stars = new string('★', rating) + new string('☆', 5 - rating),
            });
        }

        /// <summary>Ngày dự kiến nhận hàng: hôm nay + 4 đến 5 ngày, ví dụ "Nov 12–13".</summary>
        private static string Eta()
        {
            var a = DateTime.Today.AddDays(4);
            var b = DateTime.Today.AddDays(5);
            var ci = CultureInfo.GetCultureInfo("en-US");
            return a.Month == b.Month ? $"{a.ToString("MMM", ci)} {a.Day}–{b.Day}" : $"{a.ToString("MMM", ci)} {a.Day}–{b.ToString("MMM", ci)} {b.Day}";
        }
    }
}
