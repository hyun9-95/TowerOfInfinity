using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Reflection;

public class DataIntegrityChecker : EditorWindow
{
    private Vector2 scrollPos;
    private List<DataIntegrityResult> results = new List<DataIntegrityResult>();
    private bool isChecking = false;
    private Dictionary<string, Type> dataTypeMap;

    [MenuItem("Window/Data Integrity Checker")]
    public static void ShowWindow()
    {
        GetWindow<DataIntegrityChecker>("Data Integrity Checker");
    }

    private void OnEnable()
    {
        if (dataTypeMap == null)
            InitializeDataTypeMap();
    }

    private void InitializeDataTypeMap()
    {
        dataTypeMap = new Dictionary<string, Type>();
        
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        
        foreach (var assembly in assemblies)
        {
            try
            {
                var dataTypes = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && 
                                  !type.IsInterface && 
                                  typeof(IBaseData).IsAssignableFrom(type) &&
                                  type.Name.StartsWith("Data"))
                    .ToArray();

                foreach (var dataType in dataTypes)
                {
                    string typeName = dataType.Name;
                    if (typeName.StartsWith("Data"))
                    {
                        string fileName = typeName.Substring(4) + ".json";
                        dataTypeMap[fileName] = dataType;
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
                continue;
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Data Integrity Checker", EditorStyles.boldLabel);
        GUILayout.Space(10);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUI.BeginDisabledGroup(isChecking);
            if (GUILayout.Button("Check All JSON Data Integrity", GUILayout.Height(30)))
            {
                CheckDataIntegrity();
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Refresh Data Types", GUILayout.Height(30), GUILayout.Width(150)))
            {
                InitializeDataTypeMap();
                EditorGUIUtility.ExitGUI();
            }
        }

        if (dataTypeMap != null && dataTypeMap.Count > 0)
        {
            GUILayout.Label($"Detected Data Types: {dataTypeMap.Count}", EditorStyles.miniLabel);
        }

        if (isChecking)
        {
            GUILayout.Label("Checking data integrity...", EditorStyles.helpBox);
            return;
        }

        if (results.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Results ({results.Count} files checked):", EditorStyles.boldLabel);

            int successCount = results.Count(r => r.IsValid);
            int errorCount = results.Count - successCount;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label($"Success: {successCount}", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.green } });
                GUILayout.Label($"Errors: {errorCount}", new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red } });
            }

            GUILayout.Space(5);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var result in results.OrderByDescending(r => r.IsValid ? 0 : 1))
            {
                DrawResultItem(result);
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawResultItem(DataIntegrityResult result)
    {
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = result.IsValid ? Color.green * 0.3f : Color.red * 0.3f;

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUI.backgroundColor = originalColor;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(result.IsValid ? "OK" : "ERR", GUILayout.Width(30));
                GUILayout.Label(result.FileName, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label($"Records: {result.RecordCount}", EditorStyles.miniLabel);
                GUILayout.Label($"Size: {result.FileSizeKB:F1}KB", EditorStyles.miniLabel);
            }

            if (!result.IsValid && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                EditorGUILayout.HelpBox(result.ErrorMessage, MessageType.Error);
            }

            if (result.DuplicateIds.Count > 0)
            {
                EditorGUILayout.HelpBox($"Duplicate IDs found: {string.Join(", ", result.DuplicateIds)}", MessageType.Warning);
            }
        }
    }

    private void CheckDataIntegrity()
    {
        isChecking = true;
        results.Clear();

        try
        {
            string jsonPath = Path.Combine(Application.dataPath, "Data", "Jsons");
            
            if (!Directory.Exists(jsonPath))
            {
                Logger.Error($"JSON directory not found: {jsonPath}");
                isChecking = false;
                return;
            }

            string[] jsonFiles = Directory.GetFiles(jsonPath, "*.json");
            
            foreach (string filePath in jsonFiles)
            {
                string fileName = Path.GetFileName(filePath);
                CheckSingleFile(fileName, filePath);
            }

            Logger.Log($"Data integrity check completed. {results.Count(r => r.IsValid)} success, {results.Count(r => !r.IsValid)} errors.");
        }
        finally
        {
            isChecking = false;
        }
    }

    private void CheckSingleFile(string fileName, string filePath)
    {
        var result = new DataIntegrityResult
        {
            FileName = fileName,
            FileSizeKB = new FileInfo(filePath).Length / 1024f
        };

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            
            if (string.IsNullOrEmpty(jsonContent))
            {
                result.ErrorMessage = "File is empty";
                result.IsValid = false;
                results.Add(result);
                return;
            }

            if (dataTypeMap.TryGetValue(fileName, out Type dataType))
            {
                CheckDataContainerIntegrity(result, jsonContent, dataType);
            }
            else
            {
                result.ErrorMessage = $"Unknown data type for file: {fileName}";
                result.IsValid = false;
            }
        }
        catch (Exception e)
        {
            result.ErrorMessage = $"Exception: {e.Message}";
            result.IsValid = false;
        }

        results.Add(result);
    }

    private void CheckDataContainerIntegrity(DataIntegrityResult result, string jsonContent, Type dataType)
    {
        try
        {
            var containerType = typeof(DataContainer<>).MakeGenericType(dataType);
            var container = Activator.CreateInstance(containerType);
            
            var deserializeMethod = containerType.GetMethod("DeserializeJson");
            bool deserializeResult = (bool)deserializeMethod.Invoke(container, new object[] { jsonContent });
            
            if (!deserializeResult)
            {
                result.ErrorMessage = "Failed to deserialize JSON";
                result.IsValid = false;
                return;
            }

            CheckDuplicateIds(result, jsonContent);
            CountRecords(result, jsonContent);
            
            result.IsValid = true;
        }
        catch (Exception e)
        {
            result.ErrorMessage = $"DataContainer parsing failed: {e.Message}";
            result.IsValid = false;
        }
    }

    private void CheckDuplicateIds(DataIntegrityResult result, string jsonContent)
    {
        try
        {
            var jsonArray = Newtonsoft.Json.Linq.JArray.Parse(jsonContent);
            var ids = new List<int>();
            var duplicates = new HashSet<int>();

            foreach (var item in jsonArray)
            {
                if (item["id"] != null && int.TryParse(item["id"].ToString(), out int id))
                {
                    if (ids.Contains(id))
                        duplicates.Add(id);
                    else
                        ids.Add(id);
                }
            }

            result.DuplicateIds = duplicates.ToList();
        }
        catch (Exception e)
        {
            result.ErrorMessage += $" | Duplicate check failed: {e.Message}";
        }
    }

    private void CountRecords(DataIntegrityResult result, string jsonContent)
    {
        try
        {
            var jsonArray = Newtonsoft.Json.Linq.JArray.Parse(jsonContent);
            result.RecordCount = jsonArray.Count;
        }
        catch
        {
            result.RecordCount = -1;
        }
    }

    public static List<DataIntegrityResult> CheckAllJsonFilesStatic(string customJsonPath = null)
    {
        var localResults = new List<DataIntegrityResult>();
        
        try
        {
            var instance = ScriptableObject.CreateInstance<DataIntegrityChecker>();
            instance.InitializeDataTypeMap();
            
            string jsonPath = customJsonPath ?? Path.Combine(Application.dataPath, "Data", "Jsons");
            
            if (!Directory.Exists(jsonPath))
            {
                Logger.Error($"JSON directory not found: {jsonPath}");
                return localResults;
            }

            string[] jsonFiles = Directory.GetFiles(jsonPath, "*.json");
            
            foreach (string filePath in jsonFiles)
            {
                string fileName = Path.GetFileName(filePath);
                var result = instance.CheckSingleFileInternal(fileName, filePath);
                localResults.Add(result);
            }
            
            ScriptableObject.DestroyImmediate(instance);
        }
        catch (Exception e)
        {
            Logger.Error($"CheckAllJsonFiles failed: {e.Message}");
        }
        
        return localResults;
    }

    public List<DataIntegrityResult> CheckAllJsonFiles(string customJsonPath = null)
    {
        var localResults = new List<DataIntegrityResult>();
        
        try
        {
            string jsonPath = customJsonPath ?? Path.Combine(Application.dataPath, "Data", "Jsons");
            
            if (!Directory.Exists(jsonPath))
            {
                Logger.Error($"JSON directory not found: {jsonPath}");
                return localResults;
            }

            string[] jsonFiles = Directory.GetFiles(jsonPath, "*.json");
            
            foreach (string filePath in jsonFiles)
            {
                string fileName = Path.GetFileName(filePath);
                var result = CheckSingleFileInternal(fileName, filePath);
                localResults.Add(result);
            }
        }
        catch (Exception e)
        {
            Logger.Error($"CheckAllJsonFiles failed: {e.Message}");
        }
        
        return localResults;
    }

    public DataIntegrityResult CheckSingleFileInternal(string fileName, string filePath)
    {
        var result = new DataIntegrityResult
        {
            FileName = fileName,
            FileSizeKB = new FileInfo(filePath).Length / 1024f
        };

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            
            if (string.IsNullOrEmpty(jsonContent))
            {
                result.ErrorMessage = "File is empty";
                result.IsValid = false;
                return result;
            }

            if (dataTypeMap != null && dataTypeMap.TryGetValue(fileName, out Type dataType))
            {
                CheckDataContainerIntegrity(result, jsonContent, dataType);
            }
            else
            {
                result.ErrorMessage = $"Unknown data type for file: {fileName}";
                result.IsValid = false;
            }
        }
        catch (Exception e)
        {
            result.ErrorMessage = $"Exception: {e.Message}";
            result.IsValid = false;
        }

        return result;
    }

    public class DataIntegrityResult
    {
        public string FileName;
        public bool IsValid;
        public string ErrorMessage = "";
        public int RecordCount;
        public float FileSizeKB;
        public List<int> DuplicateIds = new List<int>();
    }
}