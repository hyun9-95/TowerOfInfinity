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

                    // nameId는 Define 추출용으로만 사용하고 JSON에서는 제외
                    if (name.Contains("nameId"))
                        continue;

                    if (value == "NULL")
                    {
                        dataDic.Add(name, "");
                        continue;
                    }

                    if (dataType.Contains("[]"))
                    {
                        string[] values = value.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        // ex "enum[]:CharacterDefine" → "enum:CharacterDefine"
                        System.Type arrayType = GetDataType(dataType.Replace("[]", ""));

                        Array array = Array.CreateInstance(arrayType, values.Length);
                        for (int k = 0; k < values.Length; k++)
                        {
                            object arrayValue = GetConvertValue(dataType.Replace("[]", ""), values[k]);
                            array.SetValue(arrayValue, k);
                        }
                        
                        // 중복 키 처리
                        if (dataDic.ContainsKey(name))
                        {
                            Logger.Error($"Duplicate key '{name}' found in row {i}. Skipping this column.");
                        }
                        else
                        {
                            dataDic.Add(name, array);
                        }
                    }
                    else
                    {
                        object convertValue = GetConvertValue(dataType, value);
                        
                        // 중복 키 처리
                        if (dataDic.ContainsKey(name))
                        {
                            Logger.Error($"Duplicate key '{name}' found in row {i}. Skipping this column.");
                        }
                        else
                        {
                            dataDic.Add(name, convertValue);
                        }
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
                case "bool":
                    return typeof(bool);
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
                case "bool":
                    return ConvertToBool(value);
                default:
                    return value;
            }
        }

        private bool ConvertToBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string trimmedValue = value.Trim();
            
            switch (trimmedValue)
            {
                case "TRUE":
                case "true":
                case "1":
                    return true;
                case "FALSE":
                case "false":
                case "0":
                    return false;
                default:
                    Logger.Warning($"Unable to parse bool value '{value}'. Supported formats: TRUE/true/1 for true, FALSE/false/0 for false. Defaulting to false.");
                    return false;
            }
        }
    }
}
