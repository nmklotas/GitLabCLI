using System.Collections.Generic;
using GitLabCLI.Utilities;

namespace GitLabCLI.Console.Output
{
    public sealed class RowResultFormatter
    {
        public string Format(
            string header,
            params Row[] rows)
        {
            string result = "-------------------------";
            result += "\r\n" + header;
            result += "\r\n";

            foreach (var row in rows)
            {
                result += "\r\n" + string.Join("\r\n", GetHeaderRows(row)) + "\r\n";
                result += "\r\n" + row.Body.SafeToString();
                result += "\r\n";
            }

            return result.TrimEnd();
        }

        private static IEnumerable<string> GetHeaderRows(Row row)
        {
            for (int i = 0; i < row.ColumnHeaders.Length; i++)
            {
                string header = row.ColumnHeaders[i];
                string value = row.ColumnValues[i];
                yield return $"{header}: {value}";
            }
        }
    }
}
