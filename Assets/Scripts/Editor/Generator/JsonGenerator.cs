using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Unity.VisualScripting.YamlDotNet.Core;

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

            // 중복 컬럼 분석
            var duplicateColumnInfo = AnalyzeDuplicateColumns(nameRow, dataTypeRow);

            for (int i = ValueIndex; i < sheet.Rows.Count; i++)
            {
                Dictionary<string, object> dataDic = new();

                string firstCell = sheet.Rows[i][0].ToString();

                if (string.IsNullOrWhiteSpace(firstCell))
                    continue;

                // 중복 컬럼별로 배열 생성
                var duplicateArrays = new Dictionary<string, List<object>>();

                for (int j = 0; j < sheet.Columns.Count; j++)
                {
                    string dataType = dataTypeRow[j].ToString();
                    string name = GetNaming(nameRow[j].ToString(), dataType);
                    string value = sheet.Rows[i][j].ToString();

                    // nameId는 Define 추출용으로만 사용하고 JSON에서는 제외
                    if (name.Contains("nameId"))
                        continue;

                    if (value == "NULL")
                        value = "";

                    // 중복 컬럼 처리
                    if (duplicateColumnInfo.ContainsKey(name))
                    {
                        if (!duplicateArrays.ContainsKey(name))
                            duplicateArrays[name] = new List<object>();

                        if (dataType.Contains("[]"))
                        {
                            string[] values = value.Split(';', StringSplitOptions.RemoveEmptyEntries);
                            System.Type arrayType = GetDataType(dataType.Replace("[]", ""));

                            Array array = Array.CreateInstance(arrayType, values.Length);
                            for (int k = 0; k < values.Length; k++)
                            {
                                string trimmedValue = values[k].Trim();
                                object arrayValue = GetConvertValue(dataType.Replace("[]", ""), trimmedValue);
                                array.SetValue(arrayValue, k);
                            }
                            duplicateArrays[name].Add(array);
                        }
                        else
                        {
                            string trimmedValue = value.Trim();
                            object convertValue = GetConvertValue(dataType, trimmedValue);
                            duplicateArrays[name].Add(convertValue);
                        }
                    }
                    else
                    {
                        // 일반 컬럼 처리
                        if (dataType.Contains("[]"))
                        {
                            string[] values = value.Split(';', StringSplitOptions.RemoveEmptyEntries);
                            System.Type arrayType = GetDataType(dataType.Replace("[]", ""));

                            Array array = Array.CreateInstance(arrayType, values.Length);
                            for (int k = 0; k < values.Length; k++)
                            {
                                string trimmedValue = values[k].Trim();
                                object arrayValue = GetConvertValue(dataType.Replace("[]", ""), trimmedValue);
                                array.SetValue(arrayValue, k);
                            }
                            dataDic.Add(name, array);
                        }
                        else
                        {
                            string trimmedValue = value.Trim();
                            object convertValue = GetConvertValue(dataType, trimmedValue);
                            dataDic.Add(name, convertValue);
                        }
                    }
                }

                // 중복 컬럼 배열을 JSON에 추가
                foreach (var kvp in duplicateArrays)
                {
                    dataDic.Add(kvp.Key, kvp.Value.ToArray());
                }

                dataDicList.Add(dataDic);
            }

            string newJson = JsonConvert.SerializeObject(dataDicList, Formatting.Indented);

            string assetsPath = folderPath;
            string addressablePath = folderPath.Replace("Assets/", "Assets/Addressable/");

            SaveFileAtPath(assetsPath, jsonFileName, newJson);
            SaveFileAtPath(addressablePath, jsonFileName, newJson);
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

        private Dictionary<string, int> AnalyzeDuplicateColumns(DataRow nameRow, DataRow dataTypeRow)
        {
            var duplicateInfo = new Dictionary<string, int>();
            var columnCounts = new Dictionary<string, int>();
            
            // 컬럼명 개수 세기
            for (int j = 0; j < nameRow.ItemArray.Length; j++)
            {
                string name = GetNaming(nameRow[j].ToString(), dataTypeRow[j].ToString());
                if (name.Contains("nameId"))
                    continue;
                    
                if (columnCounts.ContainsKey(name))
                    columnCounts[name]++;
                else
                    columnCounts[name] = 1;
            }
            
            // 중복된 컬럼만 저장
            foreach (var kvp in columnCounts)
            {
                if (kvp.Value > 1)
                {
                    duplicateInfo[kvp.Key] = kvp.Value;
                }
            }
            
            return duplicateInfo;
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
