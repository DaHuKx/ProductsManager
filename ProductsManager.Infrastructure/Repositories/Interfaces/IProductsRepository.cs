using ProductsManager.Domain.DbEntities;
using ProductsManager.Domain.Enums;

namespace ProductsManager.Infrastructure.Repositories.Interfaces
{
    public interface IProductsRepository : IRepositoryBase<Product>
    {
        Task<Trade?> AddTradeAsync(Trade trade);
        Task<Product?> UpdateProductPrice(int id, decimal price, TradeType type);
        Task<IEnumerable<Trade>?> AddTradesRangeAsync(IEnumerable<Trade> trades);
        Task<List<Product>> GetAllProductsWithTradesAsync();
        Task<Product?> GetProductWithTradesAsync(int id);
        Trade? RemoveTrade(int id);
    }
}
