using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Models.ViewModels;
using TDevElectronics.Services;

namespace TDevElectronics.Controllers
{
    /// <summary>
    /// Khu tài khoản khách hàng (Phần việc D, giai đoạn 1): Index, Orders, CancelRequest, Settings.
    /// Login, Register, Logout nằm ở AccountController.Auth.cs (cùng class partial).
    /// </summary>
    public partial class AccountController : Controller
    {
        // Giai đoạn 1: chưa có đơn thật nên hiển thị đơn mẫu của khách demo. Giai đoạn 2 lọc theo UserId đang đăng nhập.
        private const string DemoCustomerEmail = "alex.morgan@northstarlabs.com";

        // /Account : tổng quan, đơn hàng, yêu thích, hồ sơ (app.js đổi tab theo ?view=)
        public IActionResult Index([FromServices] IOrderService orders)
        {
            var mine = orders.GetAll()
                .Where(o => string.Equals(o.Order.Email, DemoCustomerEmail, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return View(new AccountViewModel
            {
                FullName = "Alex Morgan",
                Email = DemoCustomerEmail,
                Company = "Northstar Labs",
                JobTitle = "Procurement Manager",
                OrderCount = mine.Count,
                CancellableCount = mine.Count(o => o.CustomerCanCancel)
            });
        }

        // /Account/Orders : danh sách đơn dựng ở server, mỗi đơn có nút gửi yêu cầu hủy kèm lý do
        public IActionResult Orders([FromServices] IOrderService orders)
        {
            var mine = orders.GetAll()
                .Where(o => string.Equals(o.Order.Email, DemoCustomerEmail, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(o => o.Order.Date)
                .ToList();
            return View(mine);
        }

        // POST /Account/CancelRequest : đơn giữ nguyên trạng thái, chờ admin duyệt hoặc từ chối
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CancelRequest(CancelRequestViewModel model, [FromServices] IOrderService orders)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(Orders));
            }

            var reason = model.Reason == "Khác" ? $"Khác: {model.Detail!.Trim()}" : model.Reason;
            var r = orders.RequestCancel(model.OrderId, reason);
            TempData[r.Ok ? "Success" : "Error"] = r.Message;
            return RedirectToAction(nameof(Orders));
        }

        // /Account/Settings : giao diện sáng/tối, nhận email (app.js lưu lựa chọn)
        public IActionResult Settings() => View();
    }
}
