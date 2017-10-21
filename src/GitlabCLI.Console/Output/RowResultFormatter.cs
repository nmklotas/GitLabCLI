using System.Collections.Generic;
using System.Linq;
using System.Text;
using GitLabCLI.Utilities;
using static System.Array;
using static System.Environment;

namespace GitLabCLI.Console.Output
{
    public sealed class RowResultFormatter
    {
        public string Format(
            string header,
            params Row[] rows)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-------------------------");
            stringBuilder.AppendLine(header);
            stringBuilder.AppendLine();

            foreach (var row in rows)
            {
                if (IndexOf(rows, row) != 0)
                    stringBuilder.AppendLine();

                string headerRows = ConcatRows(GetHeaderRows(row));
                stringBuilder.AppendLine(headerRows);

                string bodyText = row.Body.SafeToString();
                if (!string.IsNullOrWhiteSpace(bodyText))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(bodyText);
                }
            }

            return stringBuilder.ToString();
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

        private static string ConcatRows(IEnumerable<string> rows)
        {
            return string.Join("\r\n", rows.Where(r => r != NewLine));
        }
    }
}
