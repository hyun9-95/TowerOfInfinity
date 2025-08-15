#if UNITY_EDITOR

using System.IO;

namespace Tools
{
    public class VersionTextGenerator : BaseGenerator
    {
        private string VersionTextName => Path.GetFileName(PathDefine.VersionText);

        public void Generate(string jsonFolderPath, string version)
        {
            string assetsPath = jsonFolderPath;
            string addressablePath = jsonFolderPath.Replace("Assets/", "Assets/Addressable/");

            SaveFileAtPath(assetsPath, VersionTextName, version);
            SaveFileAtPath(addressablePath, VersionTextName, version);
        }
    }
}
#endif
