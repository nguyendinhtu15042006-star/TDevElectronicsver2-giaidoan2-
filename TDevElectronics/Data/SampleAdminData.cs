namespace TDevElectronics.Data
{
    /// <summary>
    /// Dữ liệu mẫu cho Account + Admin (Phần việc D, giai đoạn 1). Lấy từ các bảng trong template
    /// (account.html, a-ord.html, a-quo.html). Giai đoạn 2 file này bị xóa và thay bằng OrderService / QuoteService.
    /// </summary>
    public sealed record SampleOrder(string Id, string Customer, string Company, string Email, DateTime Date,
        int Items, decimal Total, string Status, string Payment, string Address);

    public sealed record SampleQuote(string Id, string Company, string Contact, string Project, decimal Amount,
        string Status, DateTime Date, string Notes);

    public static class SampleAdminData
    {
        public static readonly string[] OrderStatuses = { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
        public static readonly string[] QuoteStatuses = { "Under review", "Quoted", "Accepted", "Expired", "Archived" };

        public static IReadOnlyList<SampleOrder> Orders { get; } = new[]
        {
            new SampleOrder("TD-1048", "Alex Morgan", "Northstar Labs", "alex.morgan@northstarlabs.com", new DateTime(2026,10,8), 3, 1541.16m, "Processing", "Visa •••• 4242", "250 Market Street, San Francisco, CA 94105"),
            new SampleOrder("TD-1047", "Alex Morgan", "Northstar Labs", "alex.morgan@northstarlabs.com", new DateTime(2026,10,2), 12, 3240.00m, "Delivered", "Visa •••• 4242", "250 Market Street, San Francisco, CA 94105"),
            new SampleOrder("TD-1046", "Sam Rivera", "Vertex Studio", "sam@vertexstudio.com", new DateTime(2026,10,7), 6, 1684.80m, "Shipped", "Bank transfer", "12 Mission Street, San Francisco, CA 94103"),
            new SampleOrder("TD-1045", "Taylor Chen", "Atlas Consulting", "taylor@atlas.com", new DateTime(2026,10,7), 15, 6210.00m, "Pending", "Net 30 invoice", "88 King Street, Seattle, WA 98104"),
            new SampleOrder("TD-1036", "Alex Morgan", "Northstar Labs", "alex.morgan@northstarlabs.com", new DateTime(2026,9,18), 8, 1944.00m, "Delivered", "Bank transfer", "250 Market Street, San Francisco, CA 94105"),
            new SampleOrder("TD-1021", "Alex Morgan", "Northstar Labs", "alex.morgan@northstarlabs.com", new DateTime(2026,9,6), 20, 5076.00m, "Cancelled", "Net 30 invoice", "250 Market Street, San Francisco, CA 94105"),
        };

        public static IReadOnlyList<SampleQuote> Quotes { get; } = new[]
        {
            new SampleQuote("Q-2084", "Northstar Labs", "Alex Morgan", "25-device rollout", 26700m, "Under review", new DateTime(2026,10,6), "25 MacBook Air M4 and USB-C hubs for a new engineering team."),
            new SampleQuote("Q-2083", "Vertex Studio", "Sam Rivera", "Creator laptop refresh", 6380m, "Under review", new DateTime(2026,10,5), "Creator laptops for the design team."),
            new SampleQuote("Q-2078", "Northstar Labs", "Alex Morgan", "Audio team upgrade", 4980m, "Quoted", new DateTime(2026,10,4), "20 × Sony WH-1000XM5."),
            new SampleQuote("Q-2065", "Northstar Labs", "Alex Morgan", "Mobile accessories", 2180m, "Accepted", new DateTime(2026,9,22), "Chargers and cables."),
            new SampleQuote("Q-2042", "Northstar Labs", "Alex Morgan", "Office furniture fit-out", 4650m, "Expired", new DateTime(2026,8,30), "Chairs and standing desks."),
        };

        public static SampleOrder? FindOrder(string? id) => Orders.FirstOrDefault(o => string.Equals(o.Id, id, StringComparison.OrdinalIgnoreCase));
        public static SampleQuote? FindQuote(string? id) => Quotes.FirstOrDefault(q => string.Equals(q.Id, id, StringComparison.OrdinalIgnoreCase));
    }
}
