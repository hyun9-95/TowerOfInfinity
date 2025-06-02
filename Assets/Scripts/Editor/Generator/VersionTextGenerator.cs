#if UNITY_EDITOR

using System.IO;

namespace Tools
{
    public class VersionTextGenerator : BaseGenerator
    {
        private string VersionTextName => Path.GetFileName(PathDefine.VersionText);

        public void Generate(string jsonFolderPath, string version)
        {
            SaveFileAtPath(jsonFolderPath, VersionTextName, version);
        }
    }
}
#endif
