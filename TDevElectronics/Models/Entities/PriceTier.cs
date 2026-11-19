namespace TDevElectronics.Models.Entities
{
    /// <summary>Bậc giá theo số lượng: từ MinQty trở lên áp dụng UnitPrice (USD). Bậc đầu có MinQty = 1.</summary>
    public class PriceTier
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int MinQty { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }
}
