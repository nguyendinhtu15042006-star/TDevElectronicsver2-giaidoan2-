namespace TDevElectronics.Models.Enums
{
    /// <summary>Trạng thái đơn hàng. Chuỗi hiển thị khớp tab lọc ở a-ord (Pending, Processing, Shipped, Delivered, Cancelled).</summary>
    public enum OrderStatus { Pending, Processing, Shipped, Delivered, Cancelled }

    public static class OrderStatusRules
    {
        /// <summary>Khách chỉ yêu cầu hủy được khi đơn còn Pending hoặc Processing.</summary>
        public static bool CustomerCanCancel(this OrderStatus s) => s is OrderStatus.Pending or OrderStatus.Processing;

        /// <summary>Admin hủy được Pending, Processing, Shipped; đơn đã giao hoặc đã hủy thì không.</summary>
        public static bool AdminCanCancel(this OrderStatus s) => s is not (OrderStatus.Delivered or OrderStatus.Cancelled);
    }
}
