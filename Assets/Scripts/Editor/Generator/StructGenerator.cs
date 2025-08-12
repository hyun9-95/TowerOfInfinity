#if UNITY_EDITOR
using System;
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

            // 중복 컬럼 정보 분석
            var duplicateColumnInfo = AnalyzeDuplicateColumns(columnNames, columnTypes);

            StringBuilder sb = new();

            sb.AppendLine(GetDataTemplate(TemplatePathDefine.StartDataTemplate, ("name", structRootName)));

            var processedColumns = new HashSet<string>();

            for (int i = 0; i < columnNames.Count; i++)
            {
                string type = columnTypes[i].Replace("enum:", "");
                string name = GetNaming(columnNames[i]);
                string modifier = GetAccessModifier(name);

                // nameId는 Define 추출용으로만 사용하고 struct에서는 제외
                if (name.Contains("nameId"))
                    continue;

                // 중복 컬럼 처리
                if (duplicateColumnInfo.ContainsKey(name))
                {
                    if (processedColumns.Contains(name))
                        continue;

                    processedColumns.Add(name);
                    GenerateDuplicateColumnStruct(sb, name, type, modifier, duplicateColumnInfo[name]);
                }
                else
                {
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
            }

            sb.AppendLine(GetDataTemplate(TemplatePathDefine.EndDateTemplate));

            SaveFileAtPath(folderPath, structFileName, sb.ToString());
        }

        private Dictionary<string, int> AnalyzeDuplicateColumns(List<string> columnNames, List<string> columnTypes)
        {
            var duplicateInfo = new Dictionary<string, int>();
            var columnCounts = new Dictionary<string, int>();
            
            // 컬럼명 개수 세기
            for (int i = 0; i < columnNames.Count; i++)
            {
                string name = GetNaming(columnNames[i]);
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

        private void GenerateDuplicateColumnStruct(StringBuilder sb, string name, string type, string modifier, int count)
        {
            string arrayType;
            string lowerName = name.ToLower();
            
            // 배열 타입인지 확인
            if (type.EndsWith("[]"))
            {
                // 2차원 배열로 변환 (CharacterDefine[] -> CharacterDefine[][])
                arrayType = type + "[]";
            }
            else
            {
                // 1차원 배열로 변환 (int -> int[])
                arrayType = type + "[]";
            }

            // struct 타입 처리
            if (type.Contains("struct:"))
            {
                arrayType = arrayType.Replace("struct:", "");
            }

            // private 배열 필드 생성
            sb.AppendLine($"    [JsonProperty(PropertyName = \"{name}\")]");
            sb.AppendLine($"    private readonly {arrayType} {lowerName};");
            sb.AppendLine();

            // Count 프로퍼티 생성
            sb.AppendLine($"    public int {name}Count => {lowerName}?.Length ?? 0;");
            sb.AppendLine();

            // Get 메서드 생성
            string returnType = type.Replace("struct:", "");
            sb.AppendLine($"    public {returnType} Get{name}(int index)");
            sb.AppendLine("    {");
            sb.AppendLine($"        return ({lowerName} != null && index >= 0 && index < {lowerName}.Length)");
            sb.AppendLine($"               ? {lowerName}[index] : {GetDefaultValue(returnType)};");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        private string GetDefaultValue(string type)
        {
            if (type.EndsWith("[]"))
                return "null";
            if (type == "int" || type == "float" || type == "double")
                return "0";
            if (type == "bool")
                return "false";
            if (type == "string")
                return "null";
            
            // enum이나 struct의 경우 기본값
            return "default";
        }

        private string GetAccessModifier(string name)
        {
            if (name.Equals("Id", StringComparison.OrdinalIgnoreCase) || 
                name.Equals("NameId", StringComparison.OrdinalIgnoreCase))
                return "private";

            return "public";
        }
    }
}
#endif
