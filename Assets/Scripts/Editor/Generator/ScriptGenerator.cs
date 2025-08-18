using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class ScriptGenerator : BaseGenerator
    {
        public enum ScriptType
        {
            MVC,
            Unit,
            Manager,
            EditorWindow,
        }

        #region SubType of Script
        public enum ManagerType
        {
            Base,
            Mono,
        }
        #endregion

        private void GenerateScript(string templatePath, string name, string saveName)
        {
            string contents = GetDataTemplate(templatePath, ("name", name));
            SaveFileAtPath(folderPath, saveName, contents);
        }

        #region GenerateScript By Type
        private void CreateFolderPath(ScriptType type, string name = null)
        {
            switch (type)
            {
                case ScriptType.MVC:
                case ScriptType.Unit:
                    folderPath = $"{PathDefine.ContentsScriptsFolderPath}/{name}";
                    break;

                case ScriptType.EditorWindow:
                    folderPath = PathDefine.EditorWindowPath;
                    break;

                case ScriptType.Manager:
                    folderPath = PathDefine.Manager;
                    break;
            }

            if (string.IsNullOrEmpty(folderPath))
            {
                Logger.Null("folder Path");
                return;
            }

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        public void Generate(ScriptType type, string name)
        {
            CreateFolderPath(type, name);

            switch (type)
            {
                case ScriptType.MVC:
                    GenerateMVC(name);
                    break;

                case ScriptType.Unit:
                    GenerateUnit(name);
                    break;

                case ScriptType.EditorWindow:
                    GenerateEditorWindow(name);
                    break;
            }

            AssetDatabase.Refresh();
        }

        public void GenerateManager(ManagerType managerType, string name)
        {
            CreateFolderPath(ScriptType.Manager);

            string templatePath = null;

            switch (managerType)
            {
                case ManagerType.Base:
                    templatePath = TemplatePathDefine.ManagerTemplate;
                    break;
                case ManagerType.Mono:
                    templatePath = TemplatePathDefine.MonoManagerTemplate;
                    break;
            }

            GenerateScript(templatePath, name, $"{name}Manager.cs");

            AssetDatabase.Refresh();
        }

        private void GenerateMVC(string name)
        {
            string controllerName = $"{name}Controller.cs";
            string modelName = $"{name}ViewModel.cs";
            string viewName = $"{name}View.cs";

            GenerateScript(TemplatePathDefine.MVC_ControllerTemplate, name, controllerName);
            GenerateScript(TemplatePathDefine.MVC_ViewModelTemplate, name, modelName);
            GenerateScript(TemplatePathDefine.MVC_ViewTemplate, name, viewName);

            RefreshUITypeEnum(name);

            CreatePrefab(name, Path.GetFileNameWithoutExtension(viewName));
        }

        private void GenerateUnit(string name)
        {
            string unitName = $"{name}Unit.cs";
            string modelName = $"{name}UnitModel.cs";

            GenerateScript(TemplatePathDefine.UnitTemplate, name, unitName);
            GenerateScript(TemplatePathDefine.UnitModelTemplate, name, modelName);

            CreatePrefab(name, Path.GetFileNameWithoutExtension(unitName));
        }

        private void GenerateEditorWindow(string name)
        {
            string EditorWindowName = $"{name}EditorWindow.cs";

            GenerateScript(TemplatePathDefine.EditorWindowTemplate, name, EditorWindowName);
        }

        private void RefreshUITypeEnum(string addName)
        {
            folderPath = PathDefine.DefinePath;
            string viewName = $"{addName}View";

            string currentEnums = string.Join(",\n\t", Enum.GetNames(typeof(UIType)));

            if (currentEnums.Contains(viewName))
                return;

            currentEnums += $",\n\t{addName}View,";

            GenerateScript(TemplatePathDefine.UITypeTemplate, currentEnums, NameDefine.UITypeDefineScriptName);
        }

        private void CreatePrefab(string folderName, string prefabName)
        {
            AssetDatabase.Refresh();

            GameObject newPrefab = new (prefabName);

            string prefabFolderPath = $"{PathDefine.UIAddressableFullPath}/{folderName}";

            if (!Directory.Exists(prefabFolderPath))
                Directory.CreateDirectory(prefabFolderPath);

            string prefabPath = $"{prefabFolderPath}/{prefabName}.prefab";

            UnityEngine.Object existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            if (existingPrefab != null)
            {
                PrefabUtility.SaveAsPrefabAssetAndConnect(newPrefab, prefabPath, InteractionMode.UserAction);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath);
            }

            if (newPrefab != null)
                UnityEngine.Object.DestroyImmediate(newPrefab);

            AssetDatabase.Refresh();
        }
        #endregion
    }
}
