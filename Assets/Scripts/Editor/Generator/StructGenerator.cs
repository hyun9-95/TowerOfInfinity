#if UNITY_EDITOR
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Tools
{
    public class StructGenerator : BaseGenerator
    {
        private string structRootName;
        private string structFileName;
        
        public void Init(string readExcelPath)
        {
            structRootName = $"Data{Path.GetFileNameWithoutExtension(readExcelPath)}";
            structFileName = structRootName + ".cs";
            folderPath = PathDefine.DataStruct;
        }

        public void Generate(DataTable sheet)
        {
            //시트에서 데이터 타입과 이름만 뽑아놓기
            List<string> columnNames = new();
            List<string> columnTypes = new();

            DataRow dataTypeRow = sheet.Rows[DataTypeIndex];
            DataRow nameRow = sheet.Rows[NameIndex];

            for (int j = 0; j < sheet.Columns.Count; j++)
            {
                string dataType = dataTypeRow[j].ToString();
                string name = nameRow[j].ToString();
                columnTypes.Add(dataType);
                columnNames.Add(name);
            }

            StringBuilder sb = new();

            sb.AppendLine(GetDataTemplate(TemplatePathDefine.StartDataTemplate, ("name", structRootName)));

            for (int i = 0; i < columnNames.Count; i++)
            {
                string type = columnTypes[i].Replace("enum:", "");
                string name = GetNaming(columnNames[i]);
                string modifier = GetAccessModifier(name);

                if (type.Contains("struct:"))
                {
                    type = type.Replace("struct:", "");
                    sb.AppendLine(GetDataTemplate(TemplatePathDefine.StructValueTemplate, ("type", type), ("name", name), ("modifier", modifier)));
                }
                else
                {
                    sb.AppendLine(GetDataTemplate(TemplatePathDefine.DataValueTemplate, ("type", type), ("name", name), ("modifier", modifier)));
                }
            }

            sb.AppendLine(GetDataTemplate(TemplatePathDefine.EndDateTemplate));

            SaveFileAtPath(folderPath, structFileName, sb.ToString());
        }

        private string GetAccessModifier(string name)
        {
            if (name.Contains("Id") || name.Contains("NameId") || name.Contains("id") || name.Contains("nameid"))
                return "private";

            return "public";
        }
    }
}
#endif
