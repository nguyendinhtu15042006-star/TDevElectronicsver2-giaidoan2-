using System.Text.Json;

namespace TDevElectronics.Helpers
{
    /// <summary>Lưu thay đổi trạng thái đơn/báo giá trong Session (giai đoạn 1). Giai đoạn 2 thay bằng bảng Order, CancelRequest, Quote.</summary>
    public static class SessionState
    {
        public static Dictionary<string, string> Get(ISession s, string key)
        {
            var json = s.GetString(key);
            return string.IsNullOrEmpty(json)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }

        public static void Remove(ISession s, string key, string id)
        {
            var d = Get(s, key);
            if (d.Remove(id)) s.SetString(key, JsonSerializer.Serialize(d));
        }

        public static void Set(ISession s, string key, string id, string value)
        {
            var d = Get(s, key);
            d[id] = value;
            s.SetString(key, JsonSerializer.Serialize(d));
        }
    }
}
