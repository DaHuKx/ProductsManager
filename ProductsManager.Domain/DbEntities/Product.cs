namespace ProductsManager.Domain.DbEntities
{
    public class Product : BaseDbEntity
    {
        public string Name { get; set; } = string.Empty;
        public List<Trade>? Trades { get; set; }
    }
}
