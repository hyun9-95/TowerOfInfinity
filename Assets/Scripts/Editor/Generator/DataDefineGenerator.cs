using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Tools
{
    public class DataDefineGenerator : BaseGenerator
    {
        private string dataName;
        private string dataDefineName;

        public void Init(string readExcelPath)
        {
            folderPath = PathDefine.DataDefinePath;
            dataName = Path.GetFileNameWithoutExtension(readExcelPath);
            dataDefineName = dataName + "Define.cs";
        }

        public void Generate(DataTable sheet)
        {
            List<Dictionary<string, object>> dataDicList = new();

            DataRow dataTypeRow = sheet.Rows[DataTypeIndex];
            DataRow nameRow = sheet.Rows[NameIndex];

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = ValueIndex; i < sheet.Rows.Count; i++)
            {
                Dictionary<string, object> dataDic = new();
                string nameId = null;
                string id = null;

                string firstCell = sheet.Rows[i][0].ToString();

                if (string.IsNullOrWhiteSpace(firstCell))
                    continue;

                for (int j = 0; j < sheet.Columns.Count; j++)
                {
                    string dataType = dataTypeRow[j].ToString();
                    string name = GetNaming(nameRow[j].ToString(), dataType);
                    string value = sheet.Rows[i][j].ToString();

                    if (name.Contains("nameId"))
                    {
                        nameId = value;
                    }
                    else if (name.Contains("id"))
                    {
                        id = value;
                    }

                    if (nameId != null && id != null)
                        break;
                }

                stringBuilder.AppendLine($"\t{nameId} = {id},");
            }

            string newJson = GetDataTemplate(TemplatePathDefine.DataDefine, ("name", dataName), ("nameList", stringBuilder.ToString()));

            SaveFileAtPath(folderPath, dataDefineName, newJson);
        }
    }
}
