using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;
using TDevElectronics.Services;

namespace TDevElectronics.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orders;
        private readonly IQuoteService _quotes;
        private readonly ICurrencyService _currency;

        public DashboardController(IOrderService orders, IQuoteService quotes, ICurrencyService currency)
        {
            _orders = orders;
            _quotes = quotes;
            _currency = currency;
        }

        public IActionResult Index()
        {
            var all = _orders.GetAll();
            var revenue = all.Where(o => o.Status != "Cancelled").Sum(o => o.Order.Total);
            return View(new DashboardViewModel
            {
                RevenueUsd = revenue,
                RevenueVnd = _currency.ToVnd(revenue),
                PendingOrders = all.Count(o => o.Status == "Pending"),
                CancelRequests = all.Count(o => o.HasPendingCancel),
                NewQuotes = _quotes.GetAll().Count(q => q.Status == "Under review"),
                LowStockProducts = SampleData.Products.Count(p => p.Stock is not null and < 5),
                RecentOrders = all.OrderByDescending(o => o.Order.Date).Take(5).Select(o => o.Order).ToList()
            });
        }
    }
}
