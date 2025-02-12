using ProductsManager.Domain.DbEntities;

namespace ProductsManager.Business.Helpers
{
    public static class TimeHelper
    {
        public static List<Product> GetTradesInInterval(DateTime start, DateTime end, List<Product> products)
        {
            foreach (var product in products)
            {
                product.Trades = product.Trades!.Where(t => t.TimeStamp >= start && t.TimeStamp <= end).ToList();
            }

            return products;
        }
    }
}
