using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationTextSupport))]
public class LocalizationTextSupportEditor : Editor
{
    private string searchString = "";
    private LocalizationDefine[] allDefines;
    private LocalizationDefine[] filteredDefines;

    private void OnEnable()
    {
        allDefines = Enum.GetValues(typeof(LocalizationDefine)).Cast<LocalizationDefine>().ToArray();
        filteredDefines = allDefines;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Localization Define Finder", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        searchString = EditorGUILayout.TextField("Search:", searchString);
        if (EditorGUI.EndChangeCheck())
        {
            FilterDefines();
        }

        if (!string.IsNullOrEmpty(searchString) && filteredDefines.Length > 0)
        {
            GUILayout.Label($"Found {filteredDefines.Length} matches:", EditorStyles.miniLabel);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            foreach (var define in filteredDefines.Take(10))
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(define.ToString(), GUILayout.Width(200));
                
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    var localizationDefineProperty = serializedObject.FindProperty("localizationDefine");
                    localizationDefineProperty.intValue = (int)define;
                    serializedObject.ApplyModifiedProperties();
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (filteredDefines.Length > 10)
            {
                EditorGUILayout.LabelField($"... and {filteredDefines.Length - 10} more", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Update Translation", GUILayout.Height(30)))
        {
            UpdateTranslation();
        }
    }

    private void FilterDefines()
    {
        if (string.IsNullOrEmpty(searchString))
        {
            filteredDefines = allDefines;
        }
        else
        {
            filteredDefines = allDefines.Where(define => 
                define.ToString().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
        }
    }

    private void UpdateTranslation()
    {
        var localizationTextSupport = (LocalizationTextSupport)target;
        
        if (localizationTextSupport == null)
            return;

        var localizationDefineProperty = serializedObject.FindProperty("localizationDefine");
        var currentDefine = (LocalizationDefine)localizationDefineProperty.intValue;

        var textMeshProProperty = serializedObject.FindProperty("textMeshPro");
        var textMeshPro = textMeshProProperty.objectReferenceValue as TMPro.TextMeshProUGUI;

        if (textMeshPro != null)
        {
            if (!EnsureDataLoaded())
            {
                Logger.Warning("Failed to load localization data!");
                return;
            }

            string translatedText = LocalizationManager.GetLocalization(currentDefine);
            textMeshPro.text = translatedText;
            
            EditorUtility.SetDirty(textMeshPro);
            Logger.Log($"Updated text to: {translatedText}");
        }
        else
        {
            Logger.Warning("TextMeshProUGUI component not found!");
        }
    }

    private bool EnsureDataLoaded()
    {
        if (DataManager.Instance.GetDataContainer<DataLocalization>() != null)
            return true;

        try
        {
            string jsonDataPath = PathDefine.Json;
            
            if (!System.IO.Directory.Exists(jsonDataPath))
            {
                Logger.Error($"JSON data path not found: {jsonDataPath}");
                return false;
            }

            string localizationJsonPath = System.IO.Path.Combine(jsonDataPath, "Localization.json");
            
            if (!System.IO.File.Exists(localizationJsonPath))
            {
                Logger.Error($"Localization.json not found at: {localizationJsonPath}");
                return false;
            }

            var dicJsonByFileName = new System.Collections.Generic.Dictionary<string, string>();
            
            try
            {
                string jsonContent = System.IO.File.ReadAllText(localizationJsonPath);
                dicJsonByFileName.Add("Localization.json", jsonContent);
                Logger.Log("Loaded Localization.json file");
            }
            catch (System.Exception fileEx)
            {
                Logger.Error($"Failed to load Localization.json: {fileEx.Message}");
                return false;
            }

            bool generateResult = DataManager.Instance.GenerateDataContainerByDataDic(dicJsonByFileName);
            
            if (!generateResult)
            {
                Logger.Error("Failed to generate data containers");
                return false;
            }

            LocalizationManager.Instance.SetLocalizationType(LocalizationType.English);
            
            Logger.Log("Data loaded successfully for editor");
            return true;
        }
        catch (System.Exception e)
        {
            Logger.Error($"Error loading data: {e.Message}");
            return false;
        }
    }
}