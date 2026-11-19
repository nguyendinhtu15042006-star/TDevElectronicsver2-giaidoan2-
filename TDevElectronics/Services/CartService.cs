using TDevElectronics.Data;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _http;
        public CartService(IHttpContextAccessor http) => _http = http;
        private ISession Session => _http.HttpContext!.Session;

        public CartViewModel Get() => CartViewModel.From(CartViewModel.Load(Session));
        public int Count => CartViewModel.Load(Session).Values.Sum();

        public CartViewModel Add(string key, int qty = 1)
        {
            var p = SampleData.Find(key);
            var items = CartViewModel.Load(Session);
            if (p is not null)
            {
                items.TryGetValue(p.Key, out var cur);
                items[p.Key] = Math.Min(CartViewModel.MaxQuantity, cur + Math.Max(1, qty));
                CartViewModel.Save(Session, items);
            }
            return Get();
        }

        public CartViewModel SetQuantity(string key, int qty)
        {
            var p = SampleData.Find(key);
            var items = CartViewModel.Load(Session);
            if (p is not null)
            {
                if (qty <= 0) items.Remove(p.Key); else items[p.Key] = Math.Min(CartViewModel.MaxQuantity, qty);
                CartViewModel.Save(Session, items);
            }
            return Get();
        }

        public CartViewModel Remove(string key) => SetQuantity(key, 0);

        public void Clear() => CartViewModel.Save(Session, new Dictionary<string, int>());
    }
}
