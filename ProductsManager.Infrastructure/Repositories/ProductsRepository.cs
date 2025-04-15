using Microsoft.EntityFrameworkCore;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Domain.Enums;
using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Infrastructure.Repositories
{
    public class ProductsRepository : RepositoryBase<Product>, IProductsRepository
    {
        #region Get
        public async Task<List<Product>> GetAllProductsWithTradesAsync()
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                return await context.Products.Include(p => p.Trades).ToListAsync();
            }
        }

        public async Task<Product?> GetProductWithTradesAsync(int id)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                return await context.Products.Include(p => p.Trades).FirstOrDefaultAsync(p => p.Id == id);
            }
        }

        public async Task<List<Trade>> GetTradesWithProductsAsync()
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                return await context.Trades.Include(t => t.Product).ToListAsync();
            }
        }
        #endregion

        #region Add
        public async Task<Trade?> AddTradeAsync(Trade trade)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                try
                {
                    await context.Trades.AddAsync(trade);
                    await context.SaveChangesAsync();

                    trade.Product = context.Products.First(p => p.Id == trade.ProductId);

                    return trade;
                }
                catch
                {
                    return null;
                }
            }
        }

        public async Task<IEnumerable<Trade>?> AddTradesRangeAsync(IEnumerable<Trade> trades)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                try
                {
                    await context.Trades.AddRangeAsync(trades);
                    await context.SaveChangesAsync();

                    return trades;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        #region Update

        public async Task<Product?> UpdateProductPrice(int id, decimal price, TradeType type)
        {
            using (ProductsManagerDb db = new ProductsManagerDb())
            {
                var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);

                if (product is null)
                {
                    return null;
                }

                if (type == TradeType.Export)
                {
                    product.ExportPrice = price;
                }
                else
                {
                    product.ImportPrice = price;
                }

                db.Update(product);
                await db.SaveChangesAsync();

                return product;
            }
        }

        #endregion

        #region Remove
        public Trade? RemoveTrade(int id)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                Trade trade;

                try
                {
                    trade = context.Trades.First(t => t.Id == id);

                    context.Remove(trade);
                    context.SaveChanges();

                    return trade;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion
    }
}
