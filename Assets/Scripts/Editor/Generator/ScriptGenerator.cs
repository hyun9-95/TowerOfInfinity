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

        private void GenerateScriptWithSuffix(string templatePath, string name, string saveName, string suffix, bool isPopup)
        {
            string uiCanvasType = isPopup ? "Popup" : "View";
            string contents = GetDataTemplate(templatePath, ("name", name), ("suffix", suffix), ("uiCanvasType", uiCanvasType));
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
                    GenerateMVC(name, false);
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

        public void GenerateMVC(string name, bool isPopup)
        {
            CreateFolderPath(ScriptType.MVC, name);
            
            string suffix = isPopup ? "Popup" : "View";
            string modelSuffix = isPopup ? "PopupModel" : "ViewModel";
            
            string controllerName = $"{name}{suffix}Controller.cs";
            string modelName = $"{name}{modelSuffix}.cs";
            string viewName = $"{name}{suffix}.cs";

            GenerateScriptWithSuffix(TemplatePathDefine.MVC_ControllerTemplate, name, controllerName, suffix, isPopup);
            GenerateScriptWithSuffix(TemplatePathDefine.MVC_ViewModelTemplate, name, modelName, suffix, isPopup);
            GenerateScriptWithSuffix(TemplatePathDefine.MVC_ViewTemplate, name, viewName, suffix, isPopup);

            RefreshUITypeEnum(name, isPopup);

            CreatePrefab(name, Path.GetFileNameWithoutExtension(viewName), isPopup);
        }

        private void GenerateUnit(string name)
        {
            string unitName = $"{name}Unit.cs";
            string modelName = $"{name}UnitModel.cs";

            GenerateScript(TemplatePathDefine.UnitTemplate, name, unitName);
            GenerateScript(TemplatePathDefine.UnitModelTemplate, name, modelName);

            CreatePrefab(name, Path.GetFileNameWithoutExtension(unitName), false);
        }

        private void GenerateEditorWindow(string name)
        {
            string EditorWindowName = $"{name}EditorWindow.cs";

            GenerateScript(TemplatePathDefine.EditorWindowTemplate, name, EditorWindowName);
        }

        private void RefreshUITypeEnum(string addName, bool isPopup)
        {
            folderPath = PathDefine.DefinePath;
            string typeName = isPopup ? $"{addName}Popup" : $"{addName}View";

            string currentEnums = string.Join(",\n\t", Enum.GetNames(typeof(UIType)));

            if (currentEnums.Contains(typeName))
                return;

            currentEnums += $",\n\t{typeName},";

            GenerateScript(TemplatePathDefine.UITypeTemplate, currentEnums, NameDefine.UITypeDefineScriptName);
        }

        private void CreatePrefab(string folderName, string prefabName, bool isPopup = false)
        {
            AssetDatabase.Refresh();

            GameObject newPrefab = new (prefabName);

            string suffix = isPopup ? "Popup" : "View";
            string folderNameWithSuffix = isPopup ? $"{folderName}Popup" : $"{folderName}View";
            string prefabFolderPath = $"{PathDefine.UIAddressableFullPath}/{folderNameWithSuffix}";

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
