using ProductsManager.Domain.Enums;

namespace ProductsManager.Domain.DbEntities
{
    public class Trade : BaseDbEntity
    {
        public int ProductId { get; set; }
        public TradeType Type { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? Comment { get; set; }
        public Product? Product { get; set; }
    }
}
