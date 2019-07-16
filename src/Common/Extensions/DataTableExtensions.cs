using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Kataclysm.Common.Reporting;

namespace Katerra.Apollo.Structures.Common.Reporting.Tabular
{
    public static class DataTableExtensions
    {
        public static IEnumerable<string> GetColumnNames(this DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                yield return column.ColumnName;
            }
        }

        public static void AddRow(this DataTable table, DataRow row)
        {
            table.Rows.Add(row);
        }
        
        /// <param name="columnName">Column Name</param>
        /// <param name="headerName">If left empty, table will convert Column Name from space or underscore delimited to Capitalized words</param>
        /// <returns>The column that was added</returns>
        public static DataColumn AddColumn(this DataTable table, string columnName, ColumnDataType dataType,
            string headerName = null)
        {
            Type type;
            
            switch (dataType)
            {
                case ColumnDataType.Text:
                    type = typeof(string);
                    break;
                case ColumnDataType.Number:
                    type = typeof(double);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            DataColumn column = CreateDataColumn(columnName, type, headerName);
            
            table.Columns.Add(column);

            return column;
        }
        
        private static DataColumn CreateDataColumn(string columnName, Type dataType, string columnHeader)
        {
            string columnCaption;
            
            if (string.IsNullOrWhiteSpace(columnHeader))
            {
                // Capitalize words, assuming words are space or underscore delimited
                string[] words = columnName.Replace(" ", "_").Split('_');

                IEnumerable<string> capitalizedWords = words.Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1).ToLower());

                columnCaption = string.Join(" ", capitalizedWords);
            }
            else
            {
                columnCaption = columnHeader;
            }
            
            return new DataColumn
            {
                ColumnName = columnName,
                Caption = columnCaption,
                Unique = false,
                DataType = dataType,
                ReadOnly = false,
                AutoIncrement = false
            };
        }
    }
}