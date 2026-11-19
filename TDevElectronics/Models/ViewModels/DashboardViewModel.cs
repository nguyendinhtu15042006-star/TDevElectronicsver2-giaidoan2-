using TDevElectronics.Data;

namespace TDevElectronics.Models.ViewModels
{
    /// <summary>Số liệu bảng điều khiển admin. Doanh thu loại trừ đơn đã hủy.</summary>
    public class DashboardViewModel
    {
        public decimal RevenueUsd { get; set; }
        public decimal RevenueVnd { get; set; }
        public int PendingOrders { get; set; }
        public int CancelRequests { get; set; }
        public int NewQuotes { get; set; }
        public int LowStockProducts { get; set; }
        public List<SampleOrder> RecentOrders { get; set; } = new();
    }
}
