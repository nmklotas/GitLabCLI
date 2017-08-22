using System;
using System.Collections.Generic;
using System.Linq;
using GitlabCmd.Console.Utilities;

namespace GitlabCmd.Console.App
{
    public sealed class GridResultFormatter
    {
        private const int _maxColumnWidth = 50;

        public string Format(string header, string[] columnHeaders, object[][] rows)
        {
            var columnWidths = GetColumnWidths(columnHeaders, rows);
            string columnsHeader = GetColumnsHeader(columnWidths, columnHeaders);
            string underlineHeader = GetHeaderUnderline(columnWidths);
            var gridRows = GetGridRows(rows, columnWidths);

            string result = "-------------------------";
            result += "\r\n" + header;
            result += "\r\n";
            result += "\r\n" + columnsHeader;
            result += "\r\n" + underlineHeader;

            foreach (string gridRow in gridRows)
                result += "\r\n" + gridRow;

            return result;
        }

        private static List<int> GetColumnWidths(string[] columnHeaders, object[][] rows)
        {
            var result = new List<int>();

            for (int i = 0; i < columnHeaders.Length; i++)
            {
                int maxTextLength = rows.Select(r => r[i].SafeToString().Length).
                    DefaultIfEmpty().
                    Max();

                int columnWidth = Math.Max(maxTextLength, columnHeaders[i].Length);
                result.Add(columnWidth);
            }

            return result;
        }

        private static string GetColumnsHeader(List<int> columnWidths, string[] columnHeaders)
        {
            string result = "";

            for (int i = 0; i < columnWidths.Count; i++)
            {
                result += "  " + EnsureLength(columnHeaders[i], columnWidths[i]);
            }

            return result.TrimStart();
        }

        private static string GetHeaderUnderline(List<int> columnWidths)
        {
            string result = "";

            foreach (int width in columnWidths)
            {
                result += "  " + '_'.Expand(width);
            }

            return result.TrimStart();
        }

        private static List<string> GetGridRows(object[][] rows, List<int> columnWidths)
        {
            var result = new List<string>();

            foreach (var row in rows)
            {
                string gridRow = "";
                for (int i = 0; i < row.Length; i++)
                    gridRow += "  " + EnsureLength(row[i].SafeToString(), columnWidths[i]);

                result.Add(gridRow.TrimStart());
            }

            return result;
        }

        private static string EnsureLength(string text, int length, int maxLength = _maxColumnWidth)
        {
            int finalLength = Math.Min(length, maxLength);
            if (text.Length == finalLength)
                return text;

            return text.Length < finalLength ?
                text.PadRight(finalLength) :
                text.Substring(0, finalLength);
        }
    }
}