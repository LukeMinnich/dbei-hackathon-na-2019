using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Kataclysm.Common.Extensions;
using Katerra.Apollo.Structures.Common.Reporting.Tabular;
using Newtonsoft.Json;

namespace Kataclysm.Common.Reporting
{
    public class Table
    {
        [JsonProperty] public DataTable Data { get; private set; }
        
        [JsonProperty] public Dictionary<string, ColumnDisplaySettings> ColumnFormattingOptions { get; private set; }
            = new Dictionary<string, ColumnDisplaySettings>(StringComparer.InvariantCultureIgnoreCase);
        
        private DataView _view;

        [JsonConstructor]
        private Table(DataTable data, Dictionary<string, ColumnDisplaySettings> columnFormattingOptions)
        {
            Data = data;
            _view = Data.DefaultView;
            
            ColumnFormattingOptions = columnFormattingOptions;
        }
        
        public Table(string tableName)
        {
            Data = new DataTable
            {
                TableName = tableName,
                CaseSensitive = false
            };

            _view = Data.DefaultView;
        }

        public void AddColumn(string columnName, ColumnDataType dataType, string headerName = null,
            int displayPrecision = 2)
        {
            DataColumn column = Data.AddColumn(columnName, dataType, headerName);
            
            ColumnFormattingOptions.Add(columnName, GetDefaultDisplaySettings(column, displayPrecision));
        }

        public void SetExpression(string columnName, string expression)
        {
            Data.Columns[columnName].Expression = expression;
        }

        public void AddRow(DataRow row)
        {
            Data.AddRow(row);
        }

        public DataRow NewRow()
        {
            return Data.NewRow();
        }

        private ColumnDisplaySettings GetDefaultDisplaySettings(DataColumn column, int displayPrecision)
        {
            if (column.DataType == typeof(string))
            {
                return new StringDisplaySettings(column.ColumnName, column.Caption);
            }

            if (column.DataType.IsNumericType())
            {
                return new NumericDisplaySettings(column.ColumnName, column.Caption, displayPrecision);
            }

            throw new InvalidDataException($"did not expect data type of {column.DataType}");
        }

        private IEnumerable<Row> RowsInView
        {
            get
            {
                List<string> columnNames = Data.GetColumnNames().ToList();

                foreach (DataRowView rowView in _view)
                {
                    var displayRow = new Row();

                    foreach (var name in columnNames)
                    {
                        displayRow.Cells.Add(Format(rowView.Row[name], ColumnFormattingOptions[name]));
                    }

                    yield return displayRow;
                }
            }
        }

        private string Format(object o, ColumnDisplaySettings formattingOptions)
        {
            if (o == null) return null;
            
            switch (formattingOptions)
            {
                case StringDisplaySettings stringOptions:
                    switch (stringOptions.Case)
                    {
                        case StringCase.Unchanged:
                            return o.ToString();
                        case StringCase.AllCaps:
                            return o.ToString().ToUpper();
                        case StringCase.AllLowercase:
                            return o.ToString().ToLower();
                        default:
                            throw new InvalidEnumArgumentException();
                    }
                case NumericDisplaySettings numericOptions:
                    return ((double) o).ToStringWithTrailingZeros(numericOptions.DecimalPrecision);
                default:
                    throw new InvalidDataException(
                        $"did not expect formatting options of type {formattingOptions.GetType()} for object of type {o.GetType()}");
            }
        }

        public string PrintToMarkdown(bool includePageBreak = false)
        {
            var sB = new StringBuilder();
            
            sB.Append("\n");
            sB.Append("\n");
            sB.Append("<center>");
            sB.Append("\n");
            sB.Append("\n");
            sB.Append($"#### {Data.TableName}");
            sB.Append("\n");
            sB.Append("\n");
            sB.Append(@"</center>");
            sB.Append("\n");
            sB.Append("\n");
            sB.Append(@"<span style=""font-size:0.75em;"">");
            sB.Append("\n");
            sB.Append("\n");
            sB.Append(GetHeadersAsMarkdownTableRow());
            sB.Append("\n");
            sB.Append(GetSeparatorAsMarkdownTableRow());
            sB.Append("\n");

            foreach (var row in RowsInView)
            {
                sB.Append(GetRowAsMarkdownTableRow(row));
                sB.Append("\n");
            }
            
            sB.Append("\n");
            sB.Append(@"</span>");
            sB.Append("\n");
            sB.Append("\n");

            if (includePageBreak) sB.Append(Html.PageBreak());

            return sB.ToString();
        }

        public void SetColumnTextCapitalization(string columnName, StringCase stringCase)
        {
            ColumnDisplaySettings formatting = GetFormattingAtColumn(columnName);

            if (formatting is StringDisplaySettings settings)
            {
                settings.Case = stringCase;
                return;
            }
            
            throw new ArgumentException("cannot set case for non-string column data");
        }

        public void SetColumnNumericDisplayPrecision(string columnName, int decimalPrecision)
        {
            ColumnDisplaySettings formatting = GetFormattingAtColumn(columnName);

            if (formatting is NumericDisplaySettings settings)
            {
                settings.DecimalPrecision = decimalPrecision;
                return;
            }
            
            throw new ArgumentException("cannot set decimal precision for non-numeric column data");
        }

        public void SetColumnAlignment(string columnName, Alignment alignment)
        {
            ColumnDisplaySettings formatting = GetFormattingAtColumn(columnName);

            formatting.Alignment = alignment;
        }

        public void SortByColumn(string columnName, SortOrder order)
        {
            _view.Sort = $"{columnName} {order.Description}";
        }

        public void SortByColumns(string primaryColumnName, SortOrder primaryOrder,
            string secondaryColumnName, SortOrder secondaryOrder)
        {
            _view.Sort = $"{primaryColumnName} {primaryOrder.Description}, {secondaryColumnName} {secondaryOrder.Description}";
        }

        public void SortByColumns(string primaryColumnName, SortOrder primaryOrder,
            string secondaryColumnName, SortOrder secondaryOrder, string tertiaryColumnName, SortOrder tertiaryOrder)
        {
            _view.Sort = $"{primaryColumnName} {primaryOrder.Description}, " +
                         $"{secondaryColumnName} {secondaryOrder.Description}, " +
                         $"{tertiaryColumnName} {tertiaryOrder.Description}";
        }

        public void SortByColumns(string primaryColumnName, SortOrder primaryOrder,
            string secondaryColumnName, SortOrder secondaryOrder, string tertiaryColumnName, SortOrder tertiaryOrder,
            string quaternaryColumnName, SortOrder quaternaryOrder)
        {
            _view.Sort = $"{primaryColumnName} {primaryOrder.Description}, " +
                         $"{secondaryColumnName} {secondaryOrder.Description}, " +
                         $"{tertiaryColumnName} {tertiaryOrder.Description}, " +
                         $"{quaternaryColumnName} {quaternaryOrder}";
        }

        public List<string> DistinctTextFromColumn(string columnName)
        {
            return TextFromColumn(columnName).Distinct().ToList();
        }

        public List<string> TextFromColumn(string columnName)
        {
            var columnIndex = Data.GetColumnNames().ToList().IndexOf(columnName);

            return RowsInView.Select(r => r.Cells[columnIndex]).ToList();
        }

        private ColumnDisplaySettings GetFormattingAtColumn(string columnName)
        {
            if (ColumnFormattingOptions.ContainsKey(columnName)) return ColumnFormattingOptions[columnName];
            
            throw new ArgumentException($"column name {columnName} not found");
        }

        private string GetHeadersAsMarkdownTableRow()
        {
            var sB = new StringBuilder("|");
            
            foreach (var column in ColumnFormattingOptions.Values)
            {
                sB.Append(column.HeaderName);

                sB.Append("|");
            }

            return sB.ToString();
        }

        private string GetSeparatorAsMarkdownTableRow()
        {
            var sB = new StringBuilder("|");
            
            foreach (var column in ColumnFormattingOptions.Values)
            {
                if (column.Alignment == Alignment.Center || column.Alignment == Alignment.Left) sB.Append(":");

                sB.Append("---");
                
                if (column.Alignment == Alignment.Center || column.Alignment == Alignment.Right) sB.Append(":");

                sB.Append("|");
            }

            return sB.ToString();
        }

        private string GetRowAsMarkdownTableRow(Row row)
        {
            var sB = new StringBuilder("|");

            row.Cells.ForEach(c => sB.Append(c + "|"));

            return sB.ToString();
        }
    }
}