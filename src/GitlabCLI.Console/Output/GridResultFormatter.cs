using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GitLabCLI.Utilities;

namespace GitLabCLI.Console.Output
{
    public sealed class GridResultFormatter
    {
        private const string _separator = "  ";

        public string Format(
            string header,
            params GridColumn[] columns)
        {
            GuardRowsAreSameLength(nameof(columns), columns);

            var calculatedColumnWidths = CalculateColumnWidths(columns).ToArray();
            var columnHeaders = GetColumnsHeaders(columns, calculatedColumnWidths);
            var underlineHeaders = GetHeaderUnderlines(calculatedColumnWidths);
            var gridRows = GetGridRows(columns, calculatedColumnWidths);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-------------------------");
            stringBuilder.AppendLine(header);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(string.Join(_separator, columnHeaders));
            stringBuilder.AppendLine(string.Join(_separator, underlineHeaders));

            foreach (string gridRow in gridRows)
                stringBuilder.AppendLine(gridRow);

            return stringBuilder.ToString();
        }

        private static IEnumerable<int> CalculateColumnWidths(GridColumn[] columns)
        {
            foreach (var column in columns)
            {
                int maxTextLength = column.Rows.Select(r => r.SafeToString().Length).
                    DefaultIfEmpty().
                    Max();

                int maxTextColumnLength = Math.Max(maxTextLength, column.Header.Length);
                int columnWidth = Math.Min(maxTextColumnLength, column.MaxColumnLength);
                yield return columnWidth;
            }
        }

        private static IEnumerable<string> GetColumnsHeaders(GridColumn[] columns, int[] columnWidths) 
            => columnWidths.Select((width, index) => EnsureLength(columns[index].Header, width));

        private static IEnumerable<string> GetHeaderUnderlines(int[] columnWidths)
            => columnWidths.Select(width => '_'.Expand(width));

        private static IEnumerable<string> GetGridRows(GridColumn[] columns, int[] columnWidths)
        {
            //GridColumns have the same rows count, so take first
            int totalRows = columns[0].Rows.Count;

            for (int row = 0; row < totalRows; row++)
            {
                string gridRow = "";

                for (int rowColumnIndex = 0; rowColumnIndex < columns.Length; rowColumnIndex++)
                {
                    var columnRow = columns[rowColumnIndex];
                    if (rowColumnIndex != 0)
                        gridRow += _separator;

                    gridRow += EnsureLength(columnRow.Rows[row].SafeToString(), columnWidths[rowColumnIndex]);
                }

                yield return gridRow.Replace("\r\n", "").Replace("\n", "");
            }
        }

        private static string EnsureLength(string text, int length)
        {
            if (text.Length == length)
                return text;

            return text.Length < length ?
                text.PadRight(length) :
                text.Substring(0, length);
        }

        private static void GuardRowsAreSameLength(string argName, params GridColumn[] columns)
        {
            if (columns.Length == 0)
                return;

            if (columns.Any(r => r.Rows.Count != columns[0].Rows.Count))
                throw new ArgumentException("All GridColumn Rows must have same length", argName);
        }
    }
}