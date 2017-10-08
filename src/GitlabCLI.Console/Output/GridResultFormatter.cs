using System;
using System.Collections.Generic;
using System.Linq;
using GitLabCLI.Utilities;

namespace GitLabCLI.Console.Output
{
    public sealed class GridResultFormatter
    {
        public string Format(string header, string[] columnHeaders, int[] maxColumnLengths, object[][] rows)
        {
            var calculatedColumnWidths = CalculateColumnWidths(columnHeaders, maxColumnLengths, rows).ToArray();
            string columnsHeader = GetColumnsHeader(calculatedColumnWidths, columnHeaders);
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

        private static IEnumerable<int> CalculateColumnWidths(string[] columnHeaders, int[] maxColumnLengths, object[][] rows)
        {
            for (int i = 0; i < columnHeaders.Length; i++)
            {
                int maxTextLength = rows.Select(r => r[i].SafeToString().Length).
                    DefaultIfEmpty().
                    Max();

                int maxTextColumnLength = Math.Max(maxTextLength, columnHeaders[i].Length);
                int columnWidth = Math.Min(maxTextColumnLength, maxColumnLengths[i]);
                yield return columnWidth;
            }
        }

        private static string GetColumnsHeader(int[] columnWidths, string[] columnHeaders)
        {
            string result = "";

            for (int i = 0; i < columnWidths.Length; i++)
                result += "  " + EnsureLength(columnHeaders[i], columnWidths[i]);

            return result.TrimStart();
        }

        private static string GetHeaderUnderline(int[] columnWidths)
        {
            string result = "";

            foreach (int width in columnWidths)
                result += "  " + '_'.Expand(width);

            return result.TrimStart();
        }

        private static IEnumerable<string> GetGridRows(object[][] rows, int[] columnWidths)
        {
            foreach (var row in rows)
            {
                string gridRow = "";
                for (int i = 0; i < row.Length; i++)
                    gridRow += "  " + EnsureLength(row[i].SafeToString(), columnWidths[i]);

                yield return gridRow.
                    TrimStart().
                    Replace("\r\n", "").
                    Replace("\n" ,"");
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
    }
}