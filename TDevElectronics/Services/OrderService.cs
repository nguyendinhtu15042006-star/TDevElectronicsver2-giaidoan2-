using TDevElectronics.Data;
using TDevElectronics.Helpers;

namespace TDevElectronics.Services
{
    /// <summary>
    /// Giai đoạn 1: đơn lấy từ SampleAdminData, thay đổi trạng thái giữ trong Session.
    /// Giai đoạn 2: đọc/ghi bảng Order, CancelRequest bằng AppDbContext.
    /// </summary>
    public class OrderService : IOrderService
    {
        private const string StatusKey = "order-status", CancelKey = "cancel-requests", RejectKey = "cancel-rejected";
        private const decimal VatRate = 0.08m;
        private readonly IHttpContextAccessor _http;

        public OrderService(IHttpContextAccessor http) => _http = http;
        private ISession Session => _http.HttpContext!.Session;

        public IReadOnlyList<OrderView> GetAll() => SampleAdminData.Orders.Select(Build).ToList();

        public OrderView? Get(string id)
        {
            var o = SampleAdminData.FindOrder(id);
            return o is null ? null : Build(o);
        }

        public decimal CalculateVat(decimal subtotal) => Math.Round(subtotal * VatRate, 2, MidpointRounding.AwayFromZero);

        public OpResult UpdateStatus(string id, string status)
        {
            var v = Get(id);
            if (v is null) return new(false, "Không tìm thấy đơn hàng.");
            if (!SampleAdminData.OrderStatuses.Contains(status)) return new(false, "Trạng thái không hợp lệ.");
            if (status == "Cancelled") return Cancel(id, null);
            if (v.Status is "Delivered" or "Cancelled") return new(false, "Đơn đã giao hoặc đã hủy thì không đổi trạng thái được.");
            SessionState.Set(Session, StatusKey, v.Order.Id, status);
            return new(true, $"Đơn {v.Order.Id} chuyển sang {status}.");
        }

        public OpResult RequestCancel(string id, string reason)
        {
            var v = Get(id);
            if (v is null) return new(false, "Không tìm thấy đơn hàng.");
            if (v.Status is not ("Pending" or "Processing")) return new(false, "Chỉ hủy được đơn đang Chờ xử lý hoặc Đang xử lý.");
            if (v.CancelReason is not null) return new(false, "Đơn này đã có yêu cầu hủy đang chờ duyệt.");
            SessionState.Remove(Session, RejectKey, v.Order.Id);
            SessionState.Set(Session, CancelKey, v.Order.Id, reason);
            return new(true, $"Đã gửi yêu cầu hủy đơn {v.Order.Id}. Admin sẽ duyệt sớm.");
        }

        public OpResult Cancel(string id, string? reason)
        {
            var v = Get(id);
            if (v is null) return new(false, "Không tìm thấy đơn hàng.");
            if (!v.AdminCanCancel) return new(false, "Đơn đã giao hoặc đã hủy thì không hủy được.");
            if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < 5) return new(false, "Hãy nhập lý do hủy (ít nhất 5 ký tự).");
            SessionState.Set(Session, StatusKey, v.Order.Id, "Cancelled");
            return new(true, $"Đã hủy đơn {v.Order.Id}.");
        }

        public OpResult ApproveCancel(string id)
        {
            var v = Get(id);
            if (v is null) return new(false, "Không tìm thấy đơn hàng.");
            if (!v.HasPendingCancel) return new(false, "Đơn này không có yêu cầu hủy đang chờ.");
            SessionState.Set(Session, StatusKey, v.Order.Id, "Cancelled");
            return new(true, $"Đã duyệt hủy đơn {v.Order.Id}.");
        }

        public OpResult RejectCancel(string id, string? reason)
        {
            var v = Get(id);
            if (v is null) return new(false, "Không tìm thấy đơn hàng.");
            if (!v.HasPendingCancel) return new(false, "Đơn này không có yêu cầu hủy đang chờ.");
            if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < 5) return new(false, "Từ chối yêu cầu phải ghi lý do (ít nhất 5 ký tự).");
            SessionState.Remove(Session, CancelKey, v.Order.Id);
            SessionState.Set(Session, RejectKey, v.Order.Id, reason.Trim());
            return new(true, $"Đã từ chối yêu cầu hủy đơn {v.Order.Id}.");
        }

        private OrderView Build(SampleOrder o)
        {
            var st = SessionState.Get(Session, StatusKey);
            var cancel = SessionState.Get(Session, CancelKey);
            var reject = SessionState.Get(Session, RejectKey);
            return new OrderView(o, st.GetValueOrDefault(o.Id, o.Status), cancel.GetValueOrDefault(o.Id), reject.GetValueOrDefault(o.Id));
        }
    }
}
