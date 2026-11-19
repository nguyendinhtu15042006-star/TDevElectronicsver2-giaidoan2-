using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Services
{
    /// <summary>Giỏ hàng lưu trong Session (giai đoạn 2 có thể lưu bảng Cart theo UserId).</summary>
    public interface ICartService
    {
        CartViewModel Get();
        int Count { get; }
        CartViewModel Add(string key, int qty = 1);
        CartViewModel SetQuantity(string key, int qty);     // qty &lt;= 0 thì xóa dòng
        CartViewModel Remove(string key);
        void Clear();
    }
}
