using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Services;

namespace TDevElectronics.Areas.Admin.Controllers
{
    /// <summary>
    /// Quản lý đơn hàng (Phần việc D, giai đoạn 1). Luồng hủy: khách gửi yêu cầu (Account/CancelRequest),
    /// admin duyệt (ApproveCancel) hoặc từ chối kèm lý do (RejectCancel); admin cũng hủy trực tiếp (Cancel) kèm lý do.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orders;
        private readonly IExportService _export;

        public OrdersController(IOrderService orders, IExportService export)
        {
            _orders = orders;
            _export = export;
        }

        public IActionResult Index() => View();

        public IActionResult Detail(string id)
        {
            var v = _orders.Get(id);
            if (v is null) return NotFound();
            ViewBag.Status = v.Status;
            ViewBag.CancelReason = v.CancelReason;
            ViewBag.RejectReason = v.RejectReason;
            return View(v.Order);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(string id, string status, string? reason)
            => Done(id, status == "Cancelled" ? _orders.Cancel(id, reason) : _orders.UpdateStatus(id, status));

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Cancel(string id, string? reason) => Done(id, _orders.Cancel(id, reason));

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ApproveCancel(string id) => Done(id, _orders.ApproveCancel(id));

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RejectCancel(string id, string? reason) => Done(id, _orders.RejectCancel(id, reason));

        // Giai đoạn 1 xuất CSV; giai đoạn 2 dùng ExportService (ClosedXML, QuestPDF)
        public IActionResult Export()
        {
            var bytes = _export.ToCsv(
                new[] { "Order", "Customer", "Company", "Date", "Items", "Total (USD)", "Status" },
                _orders.GetAll().Select(v => (IEnumerable<object?>)new object?[]
                    { v.Order.Id, v.Order.Customer, v.Order.Company, v.Order.Date, v.Order.Items, v.Order.Total, v.Status }));
            return File(bytes, "text/csv", "orders.csv");
        }

        private IActionResult Done(string id, OpResult r)
        {
            TempData[r.Ok ? "Success" : "Error"] = r.Message;
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}
