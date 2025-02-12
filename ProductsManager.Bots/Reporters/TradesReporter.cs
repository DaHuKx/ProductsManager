using ClosedXML.Excel;
using ProductsManager.Domain.DbEntities;
using ProductsManager.Domain.Enums;

namespace ProductsManager.Bots.Reporters
{
    public class TradesReporter : ReporterBase
    {
        public string? CreateTradesReport(IEnumerable<Trade> trades)
        {
            if (trades is null || !trades.Any())
            {
                return null;
            }

            var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"/Excels/TradeReport-{DateTime.UtcNow:yyyy-MM-dd HH-mm-ss}.xlsx"));

            var directory = Directory.GetParent(path);

            if (!Directory.Exists(directory.FullName))
            {
                Directory.CreateDirectory(directory.FullName);
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet("Trades");

                var tr = trades.OrderBy(t => t.TimeStamp);

                WriteColumn(worksheet, 1, "Название продукта", tr.Select(t => t.Product.Name));
                WriteColumn(worksheet, 2, "Закупка/Продажа", tr.Select(t => t.Type == TradeType.Import ? "Закупка" : "Продажа"));
                WriteColumn(worksheet, 3, "Количество", tr.Select(t => t.Count.ToString()));
                WriteColumn(worksheet, 4, "Время", tr.Select(t => t.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")));

                workbook.SaveAs(path);

                return path;
            }
        }
    }
}
