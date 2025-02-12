using ProductsManager.Domain.DbEntities;

namespace ProductsManager.Infrastructure.Repositories.Interfaces
{
    public interface IProductsRepository : IRepositoryBase<Product>
    {
        Task<Trade?> AddTradeAsync(Trade trade);
        Task<IEnumerable<Trade>?> AddTradesRangeAsync(IEnumerable<Trade> trades);
        Task<List<Product>> GetAllProductsWithTradesAsync();
        Task<Product?> GetProductWithTradesAsync(int id);
        Trade? RemoveTrade(int id);
    }
}
