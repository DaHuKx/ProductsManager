namespace ProductsManager.Domain.DbEntities
{
    public class Product : BaseDbEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal? ExportPrice { get; set; }
        public decimal? ImportPrice { get; set; }
        public List<Trade>? Trades { get; set; }
    }
}
