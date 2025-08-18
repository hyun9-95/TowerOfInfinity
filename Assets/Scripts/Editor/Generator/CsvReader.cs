#if UNITY_EDITOR
using System;
using System.Data;
using System.IO;
using System.Text;

namespace Tools
{
    public class CsvReader
    {
        public DataTable ReadCsvToDataTable(string csvFilePath)
        {
            try
            {
                if (!File.Exists(csvFilePath))
                {
                    Logger.Error($"CSV file not found: {csvFilePath}");
                    return null;
                }

                DataTable dataTable = new DataTable();
                string[] lines = File.ReadAllLines(csvFilePath, Encoding.UTF8);

                if (lines.Length == 0)
                {
                    Logger.Warning($"CSV file is empty: {csvFilePath}");
                    return null;
                }

                // CSV 구조 (DataGenerator 형식에 맞춤):
                // Line 0: Description (skip)
                // Line 1: Data types  
                // Line 2: Column names
                // Line 3+: Data

                if (lines.Length < 4)
                {
                    Logger.Error($"CSV file must have at least 4 rows (description, data types, column names, data): {csvFilePath}");
                    return null;
                }

                // Parse data from CSV lines
                string[] dataTypes = ParseCsvLine(lines[1]);
                string[] columnNames = ParseCsvLine(lines[2]);
                
                // Create DataTable columns with temporary names (DataGenerator doesn't use these)
                int maxColumns = Math.Max(dataTypes.Length, columnNames.Length);
                for (int i = 0; i < maxColumns; i++)
                {
                    dataTable.Columns.Add($"Column{i}", typeof(string));
                }

                // Add first row as placeholder (Excel files have a description row that gets ignored)
                DataRow placeholderRow = dataTable.NewRow();
                for (int j = 0; j < maxColumns; j++)
                {
                    placeholderRow[j] = j < dataTypes.Length ? "placeholder" : "";
                }
                dataTable.Rows.Add(placeholderRow);

                // Row 1: Data types (DataTypeIndex = 1)
                DataRow dataTypeRow = dataTable.NewRow();
                for (int j = 0; j < maxColumns; j++)
                {
                    dataTypeRow[j] = j < dataTypes.Length ? dataTypes[j] : "";
                }
                dataTable.Rows.Add(dataTypeRow);

                // Row 2: Column names (NameIndex = 2)
                DataRow columnNameRow = dataTable.NewRow();
                for (int j = 0; j < maxColumns; j++)
                {
                    columnNameRow[j] = j < columnNames.Length ? columnNames[j] : "";
                }
                dataTable.Rows.Add(columnNameRow);

                // Add data rows (starting from line 3, which is index 3)
                for (int i = 3; i < lines.Length; i++)
                {
                    string line = lines[i];
                    
                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] values = ParseCsvLine(line);
                    
                    // Add data to DataTable
                    DataRow row = dataTable.NewRow();
                    for (int j = 0; j < maxColumns; j++)
                    {
                        row[j] = j < values.Length ? values[j] : "";
                    }
                    
                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }
            catch (Exception e)
            {
                Logger.Exception($"Error reading CSV file: {csvFilePath}", e);
                return null;
            }
        }

        private string[] ParseCsvLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return new string[0];

            var result = new System.Collections.Generic.List<string>();
            bool inQuotes = false;
            StringBuilder currentField = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Double quote inside quoted field - add single quote
                        currentField.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        // Toggle quote mode
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // Field separator - end current field
                    result.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    // Regular character
                    currentField.Append(c);
                }
            }

            // Add the last field
            result.Add(currentField.ToString());

            return result.ToArray();
        }
    }
}
#endif