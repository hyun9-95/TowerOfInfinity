#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEngine;
namespace Tools
{
    public class JsonListGenerator : BaseGenerator
    {
        private string JsonListName => Path.GetFileName(PathDefine.JsonListText);

        public void Generate(string absoluteJsonPath, string relativeJsonPath)
        {
            string[] jsonFiles = Directory.GetFiles(absoluteJsonPath, $"*.json").Select(Path.GetFileName).ToArray();
            string jsonListDesc;

            if (jsonFiles.Length == 0)
            {
                Logger.Error("No json files in path!");
                return;
            }

            jsonListDesc = string.Join(",", jsonFiles);

            string assetsPath = relativeJsonPath;
            string addressablePath = relativeJsonPath.Replace("Assets/", "Assets/Addressable/");

            SaveFileAtPath(assetsPath, JsonListName, jsonListDesc);
            SaveFileAtPath(addressablePath, JsonListName, jsonListDesc);
        }
    }
}
#endif
