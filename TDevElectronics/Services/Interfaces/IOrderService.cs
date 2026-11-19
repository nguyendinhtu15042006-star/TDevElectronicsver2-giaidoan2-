using TDevElectronics.Data;

namespace TDevElectronics.Services
{
    public readonly record struct OpResult(bool Ok, string Message);

    /// <summary>Đơn kèm trạng thái hiện tại và các lý do hủy (nếu có).</summary>
    public sealed record OrderView(SampleOrder Order, string Status, string? CancelReason, string? RejectReason)
    {
        public bool HasPendingCancel => CancelReason is not null && Status is not ("Delivered" or "Cancelled");
        public bool CustomerCanCancel => Status is "Pending" or "Processing" && CancelReason is null;
        public bool AdminCanCancel => Status is not ("Delivered" or "Cancelled");
    }

    public interface IOrderService
    {
        IReadOnlyList<OrderView> GetAll();
        OrderView? Get(string id);
        decimal CalculateVat(decimal subtotal);
        OpResult UpdateStatus(string id, string status);
        OpResult RequestCancel(string id, string reason);   // khách gửi yêu cầu
        OpResult Cancel(string id, string? reason);          // admin hủy trực tiếp
        OpResult ApproveCancel(string id);
        OpResult RejectCancel(string id, string? reason);
    }
}
