#if UNITY_EDITOR
using System.IO;

namespace Tools
{
    public class BaseGenerator
    {
        protected string folderPath;
        protected int DataTypeIndex => 1;
        protected int NameIndex => DataTypeIndex + 1;
        protected int ValueIndex => NameIndex + 1;
        protected string GetDataTemplate(string path, params (string, string)[] args)
        {
            if (!File.Exists(path))
            {
                Logger.Warning($"There is no template at {path}");
                return null;
            }

            var template = File.ReadAllText(path);

            foreach(var arg in args)
            {
                if (!string.IsNullOrEmpty(arg.Item2))
                    template = template.Replace($"#{arg.Item1}#", arg.Item2);
            }

            return template;
        }
        protected string GetNaming(string name, string dataType = null)
        {
            //Id, NameId => id => nameId 카멜로 변경
            if (name.Contains("Id") || name.Contains("NameId"))
                name = char.ToLower(name[0]) + name[1..];

            if (!string.IsNullOrEmpty(dataType) && dataType.Contains("struct:"))
                name += "NameId";

            return name;
        }
        protected void SaveFileAtPath(string folderPath, string fileNameWithExtension, string generatedValue)
        {
            string savePath = Path.Combine(folderPath, fileNameWithExtension);

            if (string.IsNullOrEmpty(generatedValue))
            {
                Logger.Null("Generated string value");
                return;
            }

            if (File.Exists(savePath) && File.ReadAllText(savePath) == generatedValue)
                return;

            Logger.Log($"{fileNameWithExtension} {(File.Exists(savePath) ? "Edited" : "Created")}");
            File.WriteAllText(savePath, generatedValue);
        }
    }
}
#endif
