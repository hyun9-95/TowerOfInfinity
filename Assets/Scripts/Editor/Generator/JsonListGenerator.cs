#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEngine;
namespace Tools
{
    public class JsonListGenerator : BaseGenerator
    {
        private string JsonListName => Path.GetFileName(PathDefine.JsonListText);

        public void Generate(string jsonFolderPathValue)
        {
            string[] jsonFiles = Directory.GetFiles(jsonFolderPathValue, $"*.json").Select(Path.GetFileName).ToArray();
            string jsonListDesc;

            if (jsonFiles.Length == 0)
            {
                Logger.Error("No json files in path!");
                return;
            }

            jsonListDesc = string.Join(",", jsonFiles);

            SaveFileAtPath(jsonFolderPathValue, JsonListName, jsonListDesc);
        }
    }
}
#endif
