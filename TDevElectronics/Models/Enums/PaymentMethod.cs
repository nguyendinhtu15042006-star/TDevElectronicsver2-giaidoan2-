namespace TDevElectronics.Models.Enums
{
    /// <summary>9 phương thức thanh toán của Checkout (bước 2).</summary>
    public enum PaymentMethod
    {
        Cod,
        Bank,
        Net30,
        Qr,
        Card,
        Paypal,
        Apple,
        Google,
        Bnpl
    }

    /// <summary>Thông tin hiển thị / quy tắc của từng phương thức, khớp với PNAME, PSTAT và fee() trong app.js.</summary>
    public static class PaymentMethodInfo
    {
        /// <summary>Khóa dùng ở trình duyệt (data-m) cho từng phương thức.</summary>
        public static string Key(this PaymentMethod m) => m.ToString().ToLowerInvariant();

        public static string DisplayName(this PaymentMethod m) => m switch
        {
            PaymentMethod.Cod => "Pay on delivery",
            PaymentMethod.Bank => "Bank transfer",
            PaymentMethod.Net30 => "Net 30 invoice",
            PaymentMethod.Qr => "QR code",
            PaymentMethod.Card => "Card",
            PaymentMethod.Paypal => "PayPal",
            PaymentMethod.Apple => "Apple Pay",
            PaymentMethod.Google => "Google Pay",
            PaymentMethod.Bnpl => "Installments",
            _ => m.ToString()
        };

        /// <summary>Trạng thái thanh toán sau khi đặt hàng.</summary>
        public static string Status(this PaymentMethod m) => m switch
        {
            PaymentMethod.Bank => "Awaiting transfer",
            PaymentMethod.Net30 => "Invoiced · due in 30 days",
            PaymentMethod.Bnpl => "1 of 4 paid",
            PaymentMethod.Cod => "Pay on delivery",
            _ => "Paid"
        };

        /// <summary>Phụ phí thanh toán (chỉ COD thu thêm 5 USD).</summary>
        public static decimal Fee(this PaymentMethod m) => m == PaymentMethod.Cod ? 5m : 0m;

        public static bool TryParseKey(string? key, out PaymentMethod method) =>
            Enum.TryParse(key, ignoreCase: true, out method) && Enum.IsDefined(method);
    }
}
