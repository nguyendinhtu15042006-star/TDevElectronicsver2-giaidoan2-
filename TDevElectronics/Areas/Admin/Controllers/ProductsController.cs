using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;
using TDevElectronics.Services;

namespace TDevElectronics.Areas.Admin.Controllers
{
    /// <summary>Quản lý sản phẩm (Phần việc D, giai đoạn 1): dùng SampleData, chưa ghi CSDL.</summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IExportService _export;
        public ProductsController(IExportService export) => _export = export;

        public IActionResult Index() => View();

        public IActionResult Create() => View(NewForm(new ProductFormViewModel()));

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(ProductFormViewModel m)
        {
            if (SampleData.Find(m.Sku) is not null) ModelState.AddModelError(nameof(m.Sku), "SKU này đã tồn tại.");
            if (!ModelState.IsValid) return View(NewForm(m));
            TempData["Success"] = $"Đã thêm sản phẩm {m.Name} (giai đoạn 1: chưa lưu CSDL, giai đoạn 2 ghi vào bảng Product).";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            var p = SampleData.Find(id);
            if (p is null) return NotFound();
            return View(NewForm(new ProductFormViewModel
            {
                OriginalSku = p.Sku, Name = p.Name, Sku = p.Sku, CategorySlug = p.CategorySlug, Brand = p.Brand,
                Price = p.BasePrice, Currency = "USD", Stock = p.Stock ?? 0, IsActive = true, About = p.About,
                Highlights = string.Join("\n", p.Highlights),
                Specs = string.Join("\n", p.Specs.Select(s => $"{s.Label}: {s.Value}")),
                Delivery = string.Join("\n", p.Delivery)
            }));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ProductFormViewModel m)
        {
            if (SampleData.Find(id) is null) return NotFound();
            m.OriginalSku = id;
            if (!ModelState.IsValid) return View(NewForm(m));
            TempData["Success"] = $"Đã cập nhật sản phẩm {m.Name}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var p = SampleData.Find(id);
            if (p is null) return NotFound();
            TempData["Success"] = $"Đã xóa sản phẩm {p.Name} (giai đoạn 1 chỉ minh họa).";
            return RedirectToAction(nameof(Index));
        }

        // /Admin/Products/Export : giai đoạn 1 xuất CSV; giai đoạn 2 xuất xlsx/pdf (ClosedXML, QuestPDF)
        public IActionResult Export()
        {
            var bytes = _export.ToCsv(
                new[] { "Product", "SKU", "Category", "Brand", "Price (USD)", "Stock" },
                SampleData.Products.Select(p => (IEnumerable<object?>)new object?[]
                    { p.Name, p.Sku, SampleData.CategoryName(p.CategorySlug), p.Brand, p.BasePrice, p.Stock }));
            return File(bytes, "text/csv", "products.csv");
        }

        private static ProductFormViewModel NewForm(ProductFormViewModel m)
        {
            m.Categories = SampleData.Categories.Select(c => (c.Slug, c.Name)).ToList();
            return m;
        }
    }
}
