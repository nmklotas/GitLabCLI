using System;
using System.Collections.Generic;
using System.Linq;
using GitLabCLI.Utilities;

namespace GitLabCLI.Console.Output
{
    public sealed class GridResultFormatter
    {
        public string Format(
            string header,
            params GridColumn[] columns)
        {
            GuardRowsAreSameLength(nameof(columns), columns);

            var calculatedColumnWidths = CalculateColumnWidths(columns).ToArray();
            string columnsHeader = GetColumnsHeader(columns, calculatedColumnWidths);
            string underlineHeader = GetHeaderUnderline(calculatedColumnWidths);
            var gridRows = GetGridRows(columns, calculatedColumnWidths);

            string result = "-------------------------";
            result += "\r\n" + header;
            result += "\r\n";
            result += "\r\n" + columnsHeader;
            result += "\r\n" + underlineHeader;

            foreach (string gridRow in gridRows)
                result += "\r\n" + gridRow;

            return result;
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

        private static string GetColumnsHeader(GridColumn[] columns, int[] columnWidths)
        {
            string result = "";

            for (int i = 0; i < columnWidths.Length; i++)
                result += "  " + EnsureLength(columns[i].Header, columnWidths[i]);

            return result.TrimStart();
        }

        private static string GetHeaderUnderline(int[] columnWidths)
        {
            string result = "";

            foreach (int width in columnWidths)
                result += "  " + '_'.Expand(width);

            return result.TrimStart();
        }

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
                    gridRow += "  " + EnsureLength(columnRow.Rows[row].SafeToString(), columnWidths[rowColumnIndex]);
                }

                yield return gridRow.
                    TrimStart().
                    Replace("\r\n", "").
                    Replace("\n", "");
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