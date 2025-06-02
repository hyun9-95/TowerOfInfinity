using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Tools
{
    public class JsonGenerator : BaseGenerator
    {
        private string jsonFileName;

        public void Init(string readExcelPath, string jsonSavePathValue)
        {
            folderPath = jsonSavePathValue;
            jsonFileName = Path.GetFileNameWithoutExtension(readExcelPath) + ".json";
        }

        public void Generate(DataTable sheet)
        {
            List<Dictionary<string, object>> dataDicList = new();

            DataRow dataTypeRow = sheet.Rows[DataTypeIndex];
            DataRow nameRow = sheet.Rows[NameIndex];

            for (int i = ValueIndex; i < sheet.Rows.Count; i++)
            {
                Dictionary<string, object> dataDic = new();

                string firstCell = sheet.Rows[i][0].ToString();

                if (string.IsNullOrWhiteSpace(firstCell))
                    continue;

                for (int j = 0; j < sheet.Columns.Count; j++)
                {
                    string dataType = dataTypeRow[j].ToString();
                    string name = GetNaming(nameRow[j].ToString(), dataType);
                    string value = sheet.Rows[i][j].ToString();

                   

                    if (value == "NULL")
                    {
                        dataDic.Add(name, "");
                        continue;
                    }

                    if (dataType.Contains("[]"))
                    {
                        string[] values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        // ex "enum[]:CharacterDefine" → "enum:CharacterDefine"
                        System.Type arrayType = GetDataType(dataType.Replace("[]", ""));

                        Array array = Array.CreateInstance(arrayType, values.Length);
                        for (int k = 0; k < values.Length; k++)
                        {
                            object arrayValue = GetConvertValue(dataType.Replace("[]", ""), values[k]);
                            array.SetValue(arrayValue, k);
                        }
                        dataDic.Add(name, array);
                    }
                    else
                    {
                        object convertValue = GetConvertValue(dataType, value);
                        dataDic.Add(name, convertValue);
                    }
                }
                dataDicList.Add(dataDic);
            }

            string newJson = JsonConvert.SerializeObject(dataDicList, Formatting.Indented);
            SaveFileAtPath(folderPath, jsonFileName, newJson);
        }

        private System.Type GetDataType(string columnType)
        {
            switch (columnType)
            {
                case "int":
                    return typeof(int);
                case "float":
                    return typeof(float);
                default:
                    return typeof(string);
            }
        }

        private object GetConvertValue(string columnType, string value)
        {
            switch (columnType)
            {
                case "int":
                    if (int.TryParse(value, out int intValue))
                        return intValue;
                    return 0;
                case "float":
                    if (float.TryParse(value, out float floatValue))
                        return floatValue;
                    return 0f;
                default:
                    return value;
            }
        }
    }
}
