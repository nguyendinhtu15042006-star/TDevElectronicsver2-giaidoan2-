using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TDevElectronics.Areas.Admin.Controllers
{
    /// <summary>
    /// Cài đặt website (Phần việc D, giai đoạn 1): tỷ giá, banner trang chủ, sản phẩm trang chủ, thông tin website.
    /// Giao diện do app.js xử lý (localStorage); giai đoạn 2 lưu bảng SiteSetting và đọc ExchangeRate từ appsettings.json.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        public IActionResult Index() => View();
    }
}
