using System.Collections.Generic;
using System.Linq;

namespace GitLabCLI.Console.Output
{
    public class GridColumn
    {
        public GridColumn(
            string columnHeader,
            int maxColumnLength,
            IEnumerable<object> rows)
        {
            Header = columnHeader;
            MaxColumnLength = maxColumnLength;
            Rows = rows.ToArray();
        }

        public string Header { get; }

        public int MaxColumnLength { get; }

        public IReadOnlyList<object> Rows { get; protected set; }
    }

    public sealed class GridColumn<T> : GridColumn
    {
        public GridColumn(
            string columnHeader,
            int maxColumnLength,
            IEnumerable<T> rows) : base(columnHeader, maxColumnLength, rows.Cast<object>())
        {
        }
    }
}
