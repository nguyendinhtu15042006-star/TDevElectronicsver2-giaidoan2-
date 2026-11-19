using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Data;

namespace TDevElectronics.Controllers
{
    /// <summary>
    /// Trả JSON cho chatbot khách (Phần việc D, giai đoạn 1).
    /// Config: cài đặt mặc định của chatbot. Products(q): danh sách sản phẩm từ SampleData để chatbot tìm theo tên hoặc hãng
    /// ở mọi trang (kể cả trang không có thẻ sản phẩm như Cart, Checkout, Account). Giai đoạn 2 đổi sang CatalogService của B.
    /// </summary>
    [Route("Chatbot")]
    public class ChatbotController : Controller
    {
        // GET /Chatbot/Config
        [HttpGet("Config")]
        public IActionResult Config() => Json(new
        {
            on = true,
            name = "TDev Assistant",
            pos = "right",
            fx3d = true,
            tip = true,
            builtin = true,
            welcome = "",
            fallback = "",
            quick = (string[]?)null
        });

        // GET /Chatbot/Products?q=macbook   (bỏ q thì trả toàn bộ, tối đa 60 sản phẩm)
        [HttpGet("Products")]
        public IActionResult Products(string? q)
        {
            IEnumerable<SampleProduct> list = SampleData.Products;
            var term = q?.Trim();
            if (!string.IsNullOrEmpty(term))
            {
                list = list.Where(p => $"{p.Name} {p.Brand} {SampleData.CategoryName(p.CategorySlug)} {p.Spec} {p.Type}"
                    .Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            return Json(list.Take(60).Select(p => new
            {
                k = p.Key,                       // khóa sản phẩm, chatbot mở /Product/Detail/{k}
                n = p.Name,
                b = p.Brand.ToUpperInvariant(),
                p = p.PriceText,
                d = p.Spec
            }));
        }
    }
}
