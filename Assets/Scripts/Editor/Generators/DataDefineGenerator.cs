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
                    string originalName = nameRow[j].ToString();
                    string name = GetNaming(originalName, dataType);
                    string value = sheet.Rows[i][j].ToString();

                    // 원본 컬럼명에서 NameId를 찾아야 함 (대소문자 구분 없이)
                    if (originalName.Equals("NameId", StringComparison.OrdinalIgnoreCase) || 
                        originalName.Equals("nameId", StringComparison.OrdinalIgnoreCase))
                    {
                        nameId = value;
                    }
                    else if (originalName.Equals("Id", StringComparison.OrdinalIgnoreCase) || 
                             originalName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        id = value;
                    }

                    if (nameId != null && id != null)
                        break;
                }

                // nameId와 id가 유효한 경우에만 추가
                if (!string.IsNullOrWhiteSpace(nameId) && !string.IsNullOrWhiteSpace(id))
                {
                    stringBuilder.AppendLine($"\t{nameId} = {id},");
                }
            }

            string newJson = GetDataTemplate(TemplatePathDefine.DataDefine, ("name", dataName), ("nameList", stringBuilder.ToString()));

            SaveFileAtPath(folderPath, dataDefineName, newJson);
        }
    }
}
