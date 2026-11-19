using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Services;

namespace TDevElectronics.Areas.Admin.Controllers
{
    /// <summary>Quản lý yêu cầu báo giá (Phần việc D, giai đoạn 1): Index, Detail, UpdateStatus, Export.</summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class QuotesController : Controller
    {
        private readonly IQuoteService _quotes;
        private readonly IExportService _export;

        public QuotesController(IQuoteService quotes, IExportService export)
        {
            _quotes = quotes;
            _export = export;
        }

        public IActionResult Index() => View();

        public IActionResult Detail(string id)
        {
            var v = _quotes.Get(id);
            if (v is null) return NotFound();
            ViewBag.Status = v.Status;
            return View(v.Quote);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(string id, string status)
        {
            var r = _quotes.UpdateStatus(id, status);
            TempData[r.Ok ? "Success" : "Error"] = r.Message;
            return RedirectToAction(nameof(Detail), new { id });
        }

        public IActionResult Export()
        {
            var bytes = _export.ToCsv(
                new[] { "Quote", "Company", "Contact", "Project", "Before tax (USD)", "Status", "Date" },
                _quotes.GetAll().Select(v => (IEnumerable<object?>)new object?[]
                    { v.Quote.Id, v.Quote.Company, v.Quote.Contact, v.Quote.Project, v.Quote.Amount, v.Status, v.Quote.Date }));
            return File(bytes, "text/csv", "quotes.csv");
        }
    }
}
