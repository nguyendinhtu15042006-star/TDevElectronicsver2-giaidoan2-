using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Controllers
{
    /// <summary>Dữ liệu một dòng giỏ hàng gửi từ trình duyệt.</summary>
    public sealed record CartItemRequest(string Key, int Qty = 1);

    /// <summary>
    /// Giỏ hàng (Phần việc C, giai đoạn 1). Trang /Cart hiển thị giao diện giỏ; các action POST giữ bản sao
    /// giỏ trong Session (giá lấy từ SampleData, có bậc giá theo số lượng, ship và VAT) và trả JSON tổng tiền.
    /// Trình duyệt vẫn giữ giỏ trong localStorage để giao diện phản hồi ngay; cart-checkout.js đồng bộ lên đây qua Sync.
    /// </summary>
    public class CartController : Controller
    {
        // /Cart : trang giỏ hàng
        public IActionResult Index() =>
            View(CartViewModel.From(CartViewModel.Load(HttpContext.Session)));

        // POST /Cart/Add : thêm sản phẩm (cộng dồn số lượng)
        [HttpPost, IgnoreAntiforgeryToken]
        public IActionResult Add([FromBody] CartItemRequest req)
        {
            var p = SampleData.Find(req.Key);
            if (p is null) return NotFound(new { ok = false, error = "Product not found." });

            var items = CartViewModel.Load(HttpContext.Session);
            items.TryGetValue(p.Key, out var current);
            items[p.Key] = Math.Min(CartViewModel.MaxQuantity, current + Math.Max(1, req.Qty));
            return Save(items);
        }

        // POST /Cart/Update : đặt số lượng (<= 0 thì xóa dòng)
        [HttpPost, IgnoreAntiforgeryToken]
        public IActionResult Update([FromBody] CartItemRequest req)
        {
            var p = SampleData.Find(req.Key);
            if (p is null) return NotFound(new { ok = false, error = "Product not found." });

            var items = CartViewModel.Load(HttpContext.Session);
            if (req.Qty <= 0) items.Remove(p.Key);
            else items[p.Key] = Math.Min(CartViewModel.MaxQuantity, req.Qty);
            return Save(items);
        }

        // POST /Cart/Remove : xóa một sản phẩm khỏi giỏ
        [HttpPost, IgnoreAntiforgeryToken]
        public IActionResult Remove([FromBody] CartItemRequest req)
        {
            var p = SampleData.Find(req.Key);
            var items = CartViewModel.Load(HttpContext.Session);
            if (p is not null) items.Remove(p.Key);
            return Save(items);
        }

        // POST /Cart/Sync : thay toàn bộ giỏ bằng bản của trình duyệt (key → số lượng), dùng cho cart-checkout.js
        [HttpPost, IgnoreAntiforgeryToken]
        public IActionResult Sync([FromBody] Dictionary<string, int> items) =>
            Save(items ?? new Dictionary<string, int>());

        private IActionResult Save(Dictionary<string, int> items)
        {
            CartViewModel.Save(HttpContext.Session, items);
            return Json(CartViewModel.From(CartViewModel.Load(HttpContext.Session)).ToJson());
        }
    }
}
