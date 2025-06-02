#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System;
using UnityEngine;

namespace Tools
{
    public class DataContainerGeneratorGenerator : BaseGenerator
    {
        private string ContainerManagerName => Path.GetFileName(PathDefine.DataContainerGenerator);
        public DataContainerGeneratorGenerator()
        {
            folderPath = PathDefine.DataContainer;
        }
        
        public void Generate(string[] dataTypeList)
        {
            Array.Sort(dataTypeList);

            string types = string.Join(Environment.NewLine + "\t\t\t", dataTypeList.Select(dataType =>
                GetDataTemplate(TemplatePathDefine.AddContainerTypeTemplate, ("name", dataType))));

            string containerManager = GetDataTemplate(TemplatePathDefine.DataGeneratorTemplate, ("type", types));

            SaveFileAtPath(folderPath, ContainerManagerName, containerManager);
        }
    }
}
#endif
