using ClosedXML.Excel;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Domain.Enums;

namespace ProductsManager.Bots.Reporters
{
    public class ProductsReporter : ReporterBase
    {
        public string? CreateProductTradeHistory(Product product)
        {
            if (product.Trades is null || product.Trades.Count < 1)
            {
                return null;
            }

            var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"/Excels/ProductReport-{DateTime.UtcNow:yyyy-MM-dd HH-mm-ss}.xlsx"));

            var directory = Directory.GetParent(path);

            if (!Directory.Exists(directory.FullName))
            {
                Directory.CreateDirectory(directory.FullName);
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.AddWorksheet($"Report for product {product.Id}");

                var trades = product.Trades.OrderBy(t => t.TimeStamp);

                WriteColumn(worksheet, 1, "Тип торговли", trades.Select(t => t.Type == TradeType.Import ? "Закупка" : "Продажа"));
                WriteColumn(worksheet, 2, "Закуплено/Продано", trades.Select(t => t.Count.ToString()));
                WriteColumn(worksheet, 3, "Время торговли", trades.Select(t => t.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")));

                var imports = trades.Where(t => t.Type == TradeType.Import).Sum(t => t.Count);
                var exports = trades.Where(t => t.Type == TradeType.Export).Sum(t => t.Count);

                WriteColumn(worksheet, 5, "Id Продукта", [trades.First().Product!.Id.ToString()]);
                WriteColumn(worksheet, 6, "Название продукта", [trades.First().Product!.Name]);
                WriteColumn(worksheet, 7, "Остатки товара", [$"{imports - exports}шт."]);

                workbook.SaveAs(path);

                return path;
            }
        }
    }
}
