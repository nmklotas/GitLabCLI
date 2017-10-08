namespace GitLabCLI.Console.Output
{
    public sealed class GridRow
    {
        public GridRow(
            string columnHeader,
            int maxColumnLength,
            object[] rows)
        {
            ColumnHeader = columnHeader;
            MaxColumnLength = maxColumnLength;
            Rows = rows;
        }

        public string ColumnHeader { get; }

        public int MaxColumnLength { get; }

        public object[] Rows { get; }
    }
}
