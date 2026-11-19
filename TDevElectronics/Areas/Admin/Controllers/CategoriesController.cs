using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Areas.Admin.Controllers
{
    /// <summary>Quản lý danh mục (Phần việc D, giai đoạn 1).</summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Create() => View(new CategoryFormViewModel { DisplayOrder = SampleData.Categories.Count + 1 });

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(CategoryFormViewModel m)
        {
            if (SampleData.FindCategory(m.Slug) is not null) ModelState.AddModelError(nameof(m.Slug), "Slug này đã tồn tại.");
            if (!ModelState.IsValid) return View(m);
            TempData["Success"] = $"Đã thêm danh mục {m.Name}.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            var c = SampleData.FindCategory(id);
            if (c is null) return NotFound();
            var order = SampleData.Categories.ToList().FindIndex(x => x.Slug == c.Slug) + 1;
            return View(new CategoryFormViewModel { OriginalSlug = c.Slug, Name = c.Name, Slug = c.Slug, DisplayOrder = order, IsActive = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(string id, CategoryFormViewModel m)
        {
            if (SampleData.FindCategory(id) is null) return NotFound();
            m.OriginalSlug = id;
            if (!ModelState.IsValid) return View(m);
            TempData["Success"] = $"Đã cập nhật danh mục {m.Name}. Các danh mục khác tự dồn số thứ tự 1..N khi lưu CSDL.";
            return RedirectToAction(nameof(Index));
        }

        // Xóa danh mục: phải chuyển hết sản phẩm sang danh mục khác trước
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var c = SampleData.FindCategory(id);
            if (c is null) return NotFound();
            if (SampleData.CountIn(c.Slug) > 0)
                TempData["Error"] = $"Danh mục {c.Name} còn {SampleData.CountIn(c.Slug)} sản phẩm. Hãy chuyển sang danh mục khác trước.";
            else
                TempData["Success"] = $"Đã xóa danh mục {c.Name}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
