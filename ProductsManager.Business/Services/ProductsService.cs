using ProductsManager.Business.Helpers;
using ProductsManager.Business.Services.Interfaces;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Business.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsService(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _productsRepository.GetAllProductsWithTradesAsync();
        }

        public async Task<List<Product>?> GetProductsTradesForTimePeriod(DateTime start, DateTime end)
        {
            if (start > end)
            {
                return null;
            }

            var products = await _productsRepository.GetAllProductsWithTradesAsync();

            return TimeHelper.GetTradesInInterval(start, end, products);
        }

        public async Task<Product?> AddProduct(Product product)
        {
            return await _productsRepository.AddAsync(product);
        }

        public async Task<Trade?> AddTradeAsync(Trade trade)
        {
            return await _productsRepository.AddTradeAsync(trade);
        }
    }
}
