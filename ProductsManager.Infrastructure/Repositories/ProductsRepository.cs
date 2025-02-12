using Microsoft.EntityFrameworkCore;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Infrastructure.Repositories
{
    public class ProductsRepository : RepositoryBase<Product>, IProductsRepository
    {
        public ProductsRepository(ProductsManagerDb productsManager) : base(productsManager)
        {

        }

        #region Get
        public async Task<List<Product>> GetAllProductsWithTradesAsync()
        {
            return await Context.Products.Include(p => p.Trades).ToListAsync();
        }

        public async Task<Product?> GetProductWithTradesAsync(int id)
        {
            return await Context.Products.Include(p => p.Trades).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Trade>> GetTradesWithProductsAsync()
        {
            return await Context.Trades.Include(t => t.Product).ToListAsync();
        }
        #endregion

        #region Add
        public async Task<Trade?> AddTradeAsync(Trade trade)
        {
            try
            {
                await Context.Trades.AddAsync(trade);
                await Context.SaveChangesAsync();

                trade.Product = Context.Products.First(p => p.Id == trade.ProductId);

                return trade;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Trade>?> AddTradesRangeAsync(IEnumerable<Trade> trades)
        {
            try
            {
                await Context.Trades.AddRangeAsync(trades);
                await Context.SaveChangesAsync();

                return trades;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Remove
        public Trade? RemoveTrade(int id)
        {
            Trade trade;

            try
            {
                trade = Context.Trades.First(t => t.Id == id);

                Context.Remove(trade);
                Context.SaveChanges();

                return trade;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
