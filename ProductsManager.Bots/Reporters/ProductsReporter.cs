using ClosedXML.Excel;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Domain.Enums;

namespace ProductsManager.Bots.Reporters
{
    public class ProductsReporter : ReporterBase
    {
        public string? CreateProductsReport(List<Product> products)
        {
            var time = DateTime.UtcNow;

            var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"/Excels/StorageReport-{time:yyyy-MM-dd HH-mm-ss}.xlsx"));

            var directory = Directory.GetParent(path);

            if (!Directory.Exists(directory.FullName))
            {
                Directory.CreateDirectory(directory.FullName);
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.AddWorksheet($"Склад {time:dd-MM-yyyy}");

                WriteColumn(worksheet, 1, "Id Продукта", products.Select(p => p.Id.ToString()));
                WriteColumn(worksheet, 2, "Название товара", products.Select(p => p.Name));
                WriteColumn(worksheet, 3, "Остаток на складе", products.Select(p => GetStockForProduct(p).ToString()));
                WriteColumn(worksheet, 4, "Цена закупки", products.Select(p => p.ImportPrice?.ToString() ?? "Не указано"));
                WriteColumn(worksheet, 5, "Цена продажи", products.Select(p => p.ExportPrice?.ToString() ?? "Не указано"));

                workbook.SaveAs(path);

                return path;
            }
        }

        private int GetStockForProduct(Product product)
        {
            if (product.Trades is null ||
                product.Trades.Count == 0)
            {
                return 0;
            }

            var imports = product.Trades.Where(t => t.Type == TradeType.Import);

            if (!imports.Any())
            {
                return 0;
            }

            var exports = product.Trades.Where(t => t.Type == TradeType.Export);

            if (!exports.Any())
            {
                return imports.Count();
            }

            return imports.Sum(i => i.Count) - exports.Sum(e => e.Count);
        }
    }
}
