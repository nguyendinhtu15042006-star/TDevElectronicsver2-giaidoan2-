using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using TDevElectronics.Helpers;
using TDevElectronics.Models.Enums;
using TDevElectronics.Models.ViewModels;

namespace TDevElectronics.Controllers
{
    /// <summary>
    /// Thanh toán 4 bước (Phần việc C, giai đoạn 1): địa chỉ, giao hàng, thanh toán, xác nhận.
    /// Giai đoạn 1 chưa có CSDL nên đơn vừa đặt được giữ trong Session để trang Success hiển thị.
    /// </summary>
    public class CheckoutController : Controller
    {
        private const string LastOrderKey = "tdev.lastOrder";
        private const string CounterKey = "tdev.orderCounter";
        private static readonly Regex RefPattern = new(@"^TD-\d{3,8}$", RegexOptions.Compiled);
        private static readonly string[] Months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        // /Checkout : trang thanh toán (giao diện 4 bước do app.js + cart-checkout.js điều khiển)
        public IActionResult Index() => View(new CheckoutViewModel
        {
            Cart = CartViewModel.From(CartViewModel.Load(HttpContext.Session))
        });

        // POST /Checkout/PlaceOrder : kiểm tra địa chỉ và giỏ hàng phía server rồi ghi nhận đơn
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult PlaceOrder([FromBody] PlaceOrderRequest req)
        {
            if (req is null) return BadRequest(new { ok = false, errors = new Dictionary<string, string> { ["form"] = "Invalid request." } });
            var errors = Validate(req);
            // Lỗi từ DataAnnotations của PlaceOrderRequest (không ghi đè lỗi nghiệp vụ đã có ở Validate)
            foreach (var (field, state) in ModelState)
            {
                if (state.Errors.Count == 0) continue;
                var key = field == nameof(PlaceOrderRequest.CountryCode) ? "country" : char.ToLowerInvariant(field[0]) + field[1..];
                if (!errors.ContainsKey(key)) errors[key] = state.Errors[0].ErrorMessage;
            }

            if (!PaymentMethodInfo.TryParseKey(req.Payment, out var method))
                errors["payment"] = "Choose a payment method.";

            if (!string.IsNullOrWhiteSpace(req.Ref) && !RefPattern.IsMatch(req.Ref))
                errors["ref"] = "Invalid order reference.";

            // Ưu tiên giỏ trình duyệt gửi kèm (đã chụp lúc bấm Place Order), nếu không có thì dùng giỏ trong Session.
            var source = req.Items.Count > 0
                ? req.Items.GroupBy(i => i.Key).ToDictionary(g => g.Key, g => g.Sum(i => i.Qty))
                : CartViewModel.Load(HttpContext.Session);
            var cart = CartViewModel.From(source);
            if (cart.IsEmpty) errors["cart"] = "Your cart is empty.";

            if (errors.Count > 0) return BadRequest(new { ok = false, errors });

            var fee = method.Fee();
            var orderRef = string.IsNullOrWhiteSpace(req.Ref) ? NextRef() : req.Ref!;
            BumpCounter(orderRef);

            var country = CountryData.Find(req.CountryCode)!;
            var address = new Models.Entities.Address
            {
                FullName = req.FullName.Trim(),
                Company = req.Company?.Trim(),
                Email = req.Email.Trim(),
                Phone = req.Phone.Trim(),
                CountryCode = country.Code,
                Line1 = req.Line1.Trim(),
                Line2 = req.Line2?.Trim(),
                Ward = req.Ward?.Trim(),
                District = req.District?.Trim(),
                City = string.IsNullOrWhiteSpace(req.City) ? (req.Region ?? "").Trim() : req.City.Trim(),
                Region = req.Region?.Trim(),
                PostalCode = req.PostalCode?.Trim()
            };

            var order = new OrderSummary
            {
                Ref = orderRef,
                PlacedAt = DateTime.Now,
                Name = address.FullName,
                Email = address.Email,
                ShipTo = $"{address.OneLine()}, {country.NameEn}",
                PaymentKey = method.Key(),
                PaymentName = method.DisplayName(),
                PaymentStatus = method.Status(),
                Eta = EtaText(),
                Subtotal = cart.Subtotal,
                Shipping = cart.Shipping,
                Tax = cart.Tax,
                Fee = fee,
                Total = cart.Total + fee,
                Lines = cart.Lines.Select(l => new OrderSummaryLine
                {
                    Name = l.Name, Sku = l.Sku, Qty = l.Quantity, Unit = l.UnitPrice, Line = l.LineTotal
                }).ToList()
            };

            HttpContext.Session.SetString(LastOrderKey, JsonSerializer.Serialize(order));
            CartViewModel.Save(HttpContext.Session, new Dictionary<string, int>()); // đặt xong thì làm trống giỏ

            return Json(new
            {
                ok = true,
                orderRef = order.Ref,
                total = order.Total,
                payment = order.PaymentName,
                status = order.PaymentStatus,
                eta = order.Eta,
                successUrl = Url.Action("Success", "Checkout", new { id = order.Ref })
            });
        }

        // /Checkout/Success : xác nhận đơn vừa đặt
        public IActionResult Success(string? id)
        {
            var raw = HttpContext.Session.GetString(LastOrderKey);
            OrderSummary? order = null;
            if (!string.IsNullOrEmpty(raw))
            {
                try { order = JsonSerializer.Deserialize<OrderSummary>(raw); }
                catch (JsonException) { order = null; }
            }
            if (order is null) return RedirectToAction("Index", "Cart");
            return View(order);
        }

        // ---------------- kiểm tra dữ liệu ----------------

        private static Dictionary<string, string> Validate(PlaceOrderRequest r)
        {
            var e = new Dictionary<string, string>();
            var cc = (r.CountryCode ?? "").Trim().ToUpperInvariant();

            if ((r.FullName ?? "").Trim().Length < 2) e["fullName"] = "Enter your full name.";
            if (!new EmailAddressAttribute().IsValid((r.Email ?? "").Trim())) e["email"] = "Enter a valid email address.";
            if (!CountryData.IsKnownCountry(cc))
            {
                e["country"] = "Choose a country.";
                return e;
            }
            if (!CountryData.IsValidPhone(cc, r.Phone))
                e["phone"] = cc == "VN" ? "Enter a valid Vietnamese phone number, e.g. 0912 345 678."
                                        : "Enter a valid phone number, including the country code.";

            if (string.IsNullOrWhiteSpace(r.Line1)) e["line1"] = "Enter your street address.";
            if (cc == "VN" && string.IsNullOrWhiteSpace(r.Ward)) e["ward"] = "Enter your ward / commune.";
            if (cc != "VN" && string.IsNullOrWhiteSpace(r.City)) e["city"] = "Enter your city.";

            if (CountryData.IsRegionRequired(cc) && string.IsNullOrWhiteSpace(r.Region))
                e["region"] = "Choose or enter your state / province.";
            else
            {
                var list = CountryData.Regions(cc);
                if (list.Count > 0 && !string.IsNullOrWhiteSpace(r.Region) &&
                    !list.Any(x => x.Code.Equals(r.Region!.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                   x.Name.Equals(r.Region!.Trim(), StringComparison.OrdinalIgnoreCase)))
                    e["region"] = "Choose a state / province from the list.";
            }

            if (!CountryData.IsValidPostalCode(cc, r.PostalCode))
            {
                var rule = CountryData.PostalRuleFor(cc);
                e["postalCode"] = string.IsNullOrWhiteSpace(r.PostalCode)
                    ? "Enter your postal code."
                    : $"Enter a valid postal code ({rule?.Hint}).";
            }
            return e;
        }

        private string NextRef()
        {
            var n = HttpContext.Session.GetInt32(CounterKey) ?? 1048;
            return $"TD-{n + 1}";
        }

        private void BumpCounter(string orderRef)
        {
            if (int.TryParse(orderRef.AsSpan(3), NumberStyles.None, CultureInfo.InvariantCulture, out var n))
            {
                var cur = HttpContext.Session.GetInt32(CounterKey) ?? 1048;
                HttpContext.Session.SetInt32(CounterKey, Math.Max(cur, n));
            }
        }

        /// <summary>Ngày giao dự kiến: 4 đến 5 ngày nữa, định dạng giống ETA trong app.js (ví dụ "Nov 13–14").</summary>
        private static string EtaText()
        {
            var a = DateTime.Today.AddDays(4);
            var b = DateTime.Today.AddDays(5);
            return $"{Months[a.Month - 1]} {a.Day}–{(a.Month == b.Month ? "" : Months[b.Month - 1] + " ")}{b.Day}";
        }
    }
}
