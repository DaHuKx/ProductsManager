using ClosedXML.Excel;

namespace ProductsManager.Bots.Reporters
{
    public abstract class ReporterBase
    {
        protected void WriteColumn(IXLWorksheet worksheet, int columnNumber, string name, IEnumerable<string> values)
        {
            var column = worksheet.Column(columnNumber);

            column.Width = Math.Max(name.Length, values.Max(v => v.ToString().Length)) + 2;

            var cell = worksheet.Cell(1, columnNumber);

            cell.Value = name;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            foreach (var value in values)
            {
                cell = cell.CellBelow();
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                cell.Value = value;
            }
        }
    }
}
