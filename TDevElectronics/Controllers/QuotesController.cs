using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Models.ViewModels;
using TDevElectronics.Services;

namespace TDevElectronics.Controllers
{
    /// <summary>Báo giá của khách (Phần việc D): Rfq, Index, Detail.</summary>
    public class QuotesController : Controller
    {
        private readonly IQuoteService _quotes;
        public QuotesController(IQuoteService quotes) => _quotes = quotes;

        // /Quotes/Rfq : form yêu cầu báo giá (menu "Request a Quote")
        [HttpGet]
        public IActionResult Rfq() => View();

        // POST /Quotes/Rfq : kiểm tra dữ liệu rồi tạo yêu cầu. Form tĩnh của template do app.js gửi; action này dùng khi form bind RfqViewModel
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Rfq(RfqViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View();
            }
            var code = _quotes.Submit(model);
            TempData["Success"] = $"Đã gửi yêu cầu báo giá {code}. Chúng tôi phản hồi trong 1 ngày làm việc.";
            return RedirectToAction(nameof(Detail), new { id = code });
        }

        // /Quotes : danh sách báo giá của tôi (sau khi bấm Submit Request)
        public IActionResult Index() => View();

        // /Quotes/Detail/Q-2078 : chi tiết một báo giá. Mã vừa tạo (chưa có trong dữ liệu mẫu) hiển thị trạng thái Under review
        public IActionResult Detail(string id)
        {
            var q = _quotes.Get(id);
            if (q is null && !System.Text.RegularExpressions.Regex.IsMatch(id ?? "", @"^Q-\d{4,}$")) return NotFound();
            ViewBag.Code = id;
            return View(q);
        }
    }
}
