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
            params GridRow[] rows)
        {
            GuardRowsAreSameLength(nameof(rows), rows);

            var calculatedColumnWidths = CalculateColumnWidths(rows).ToArray();
            string columnsHeader = GetColumnsHeader(rows, calculatedColumnWidths);
            string underlineHeader = GetHeaderUnderline(calculatedColumnWidths);
            var gridRows = GetGridRows(rows, calculatedColumnWidths);

            string result = "-------------------------";
            result += "\r\n" + header;
            result += "\r\n";
            result += "\r\n" + columnsHeader;
            result += "\r\n" + underlineHeader;

            foreach (string gridRow in gridRows)
                result += "\r\n" + gridRow;

            return result;
        }

        private static IEnumerable<int> CalculateColumnWidths(GridRow[] rows)
        {
            foreach (var row in rows)
            {
                int maxTextLength = row.Rows.Select(r => r.SafeToString().Length).
                    DefaultIfEmpty().
                    Max();

                int maxTextColumnLength = Math.Max(maxTextLength, row.ColumnHeader.Length);
                int columnWidth = Math.Min(maxTextColumnLength, row.MaxColumnLength);
                yield return columnWidth;
            }
        }

        private static string GetColumnsHeader(GridRow[] rows, int[] columnWidths)
        {
            string result = "";

            for (int i = 0; i < columnWidths.Length; i++)
                result += "  " + EnsureLength(rows[i].ColumnHeader, columnWidths[i]);

            return result.TrimStart();
        }

        private static string GetHeaderUnderline(int[] columnWidths)
        {
            string result = "";

            foreach (int width in columnWidths)
                result += "  " + '_'.Expand(width);

            return result.TrimStart();
        }

        private static IEnumerable<string> GetGridRows(GridRow[] rows, int[] columnWidths)
        {
            int totalRows = rows[0].Rows.Length;
            for (int i = 0; i < totalRows; i++)
            {
                string gridRow = "";

                for (int columRowIndex = 0; columRowIndex < rows.Length; columRowIndex++)
                {
                    var columnRow = rows[columRowIndex];
                    gridRow += "  " + EnsureLength(columnRow.Rows[i].SafeToString(), columnWidths[columRowIndex]);
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

        private static void GuardRowsAreSameLength(string argName, params GridRow[] rows)
        {
            if (rows.Length == 0)
                return;

            if (rows.Any(r => r.Rows.Length != rows[0].Rows.Length))
                throw new ArgumentException("All GridRow Rows must have same length", argName);
        }
    }
}