using System;

namespace GitLabCLI.Console.Output
{
    public sealed class Row
    {
        public Row(
            string[] columnHeaders,
            string[] columnValues,
            object body)
        {
            if (columnHeaders.Length != columnValues.Length)
                throw new ArgumentException("columnHeaders, columnValues, must have same length");

            ColumnHeaders = columnHeaders;
            ColumnValues = columnValues;
            Body = body;
        }

        public string[] ColumnHeaders { get; }

        public string[] ColumnValues { get; }

        public object Body { get; }
    }
}