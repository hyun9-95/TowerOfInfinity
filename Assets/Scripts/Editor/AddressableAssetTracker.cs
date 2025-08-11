using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AddressableAssetTracker
{
    private const string ADDRESSABLE_ROOT = "Assets/Addressable";
    private const string UNTRACKED_FOLDER = "Assets/Addressable/Untracked";
    
    [MenuItem("Tools/Move Untracked Assets to Addressable Folder")]
    public static void MoveUntrackedAssetsToAddressable()
    {
        var prefabAssetMap = new Dictionary<string, HashSet<string>>();
        
        try
        {
            EditorUtility.DisplayProgressBar("Scanning Assets", "Finding Addressable prefabs...", 0f);
            
            var addressablePrefabs = ScanAddressablePrefabs();
            Logger.Log($"Found {addressablePrefabs.Count} prefabs in Addressable folder");
            
            int currentIndex = 0;
            foreach (var prefabPath in addressablePrefabs)
            {
                float progress = (float)currentIndex / addressablePrefabs.Count;
                EditorUtility.DisplayProgressBar("Scanning Dependencies", $"Processing {Path.GetFileName(prefabPath)}", progress);
                
                var dependencies = GetAssetDependencies(prefabPath);
                var untrackedDependencies = new HashSet<string>();
                
                foreach (var dependency in dependencies)
                {
                    if (IsAssetUntracked(dependency))
                    {
                        untrackedDependencies.Add(dependency);
                    }
                }
                
                if (untrackedDependencies.Count > 0)
                {
                    string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                    prefabAssetMap[prefabName] = untrackedDependencies;
                }
                
                currentIndex++;
            }
            
            int totalAssets = prefabAssetMap.Values.SelectMany(x => x).Distinct().Count();
            Logger.Log($"Found {totalAssets} untracked assets across {prefabAssetMap.Count} prefabs");
            
            if (totalAssets > 0)
            {
                MoveAssetsByPrefab(prefabAssetMap);
                Logger.Success($"Successfully processed {prefabAssetMap.Count} prefabs with untracked assets");
            }
            else
            {
                Logger.Log("No untracked assets found");
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Error during asset tracking: {e.Message}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
    
    private static List<string> ScanAddressablePrefabs()
    {
        var prefabPaths = new List<string>();
        
        if (!Directory.Exists(ADDRESSABLE_ROOT))
        {
            Logger.Warning($"Addressable root folder not found: {ADDRESSABLE_ROOT}");
            return prefabPaths;
        }
        
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { ADDRESSABLE_ROOT });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            prefabPaths.Add(path);
        }
        
        return prefabPaths;
    }
    
    private static List<string> GetAssetDependencies(string assetPath)
    {
        var dependencies = new List<string>();
        
        string[] dependencyPaths = AssetDatabase.GetDependencies(assetPath, true);
        
        foreach (string dependency in dependencyPaths)
        {
            if (dependency != assetPath && !IsScriptAsset(dependency))
            {
                dependencies.Add(dependency);
            }
        }
        
        return dependencies;
    }
    
    private static bool IsAssetUntracked(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            return false;
            
        if (assetPath.StartsWith("Packages/"))
            return false;
            
        if (assetPath.StartsWith("Resources/") || assetPath.Contains("/Resources/"))
            return false;
            
        if (!assetPath.StartsWith("Assets/"))
            return false;
            
        if (assetPath.StartsWith(ADDRESSABLE_ROOT))
            return false;
            
        if (IsScriptAsset(assetPath))
            return false;
            
        if (IsBuiltInAsset(assetPath))
            return false;
            
        if (IsTextMeshProAsset(assetPath))
            return false;
            
        return true;
    }
    
    private static bool IsScriptAsset(string assetPath)
    {
        if (assetPath.StartsWith("Assets/Scripts/"))
            return true;
            
        string extension = Path.GetExtension(assetPath).ToLower();
        return extension == ".cs" || extension == ".js" || extension == ".dll";
    }
    
    private static bool IsBuiltInAsset(string assetPath)
    {
        return assetPath.StartsWith("Library/unity") || 
               assetPath.Contains("unity_builtin_") ||
               assetPath == "Resources/unity_builtin_default";
    }
    
    private static bool IsTextMeshProAsset(string assetPath)
    {
        // TextMeshPro 관련 경로 확인
        if (assetPath.Contains("TextMeshPro") || assetPath.Contains("TextMesh Pro"))
            return true;
            
        // TMP 관련 폰트 및 에셋
        if (assetPath.Contains("/TMP/") || assetPath.EndsWith(".tmp"))
            return true;
            
        // TextMeshPro 폰트 에셋
        string extension = Path.GetExtension(assetPath).ToLower();
        if (extension == ".asset")
        {
            string fileName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
            if (fileName.Contains("tmp") || fileName.Contains("textmeshpro"))
                return true;
        }
        
        return false;
    }
    
    private static void CreateUntrackedFolder()
    {
        if (!AssetDatabase.IsValidFolder(UNTRACKED_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets/Addressable", "Untracked");
            AssetDatabase.Refresh();
            Logger.Log($"Created Untracked folder: {UNTRACKED_FOLDER}");
        }
    }
    
    private static void MoveAssetsByPrefab(Dictionary<string, HashSet<string>> prefabAssetMap)
    {
        int totalMoved = 0;
        int currentPrefab = 0;
        
        // 에셋 이동 시작
        foreach (var kvp in prefabAssetMap)
        {
            string prefabName = kvp.Key;
            var assetPaths = kvp.Value;
            
            float progress = (float)currentPrefab / prefabAssetMap.Count;
            EditorUtility.DisplayProgressBar("Moving Assets", $"Processing prefab: {prefabName}", progress);
            
            int movedForPrefab = 0;
            string targetFolderPath = GetUntrackedFolderForPrefab(prefabName);
            
            foreach (string assetPath in assetPaths)
            {
                if (MoveAssetToPrefabFolder(assetPath, targetFolderPath))
                {
                    movedForPrefab++;
                    totalMoved++;
                }
            }
            
            if (movedForPrefab > 0)
            {
                Logger.Log($"Moved {movedForPrefab} assets for prefab: {prefabName} to {targetFolderPath}");
            }
            else
            {
                Logger.Log($"No assets moved for prefab: {prefabName} (all assets may have failed to move)");
            }
            
            currentPrefab++;
        }
        
        AssetDatabase.Refresh();
        
        // 빈 폴더 정리
        RemoveEmptyFolders();
        
        Logger.Log($"Total moved {totalMoved} assets across {prefabAssetMap.Count} prefab folders");
    }
    
    private static void RemoveEmptyFolders()
    {
        if (!AssetDatabase.IsValidFolder(UNTRACKED_FOLDER))
            return;
            
        string[] subFolders = AssetDatabase.GetSubFolders(UNTRACKED_FOLDER);
        int removedCount = 0;
        
        foreach (string folder in subFolders)
        {
            string[] filesInFolder = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            // .meta 파일 제외하고 실제 에셋 파일만 확인
            bool hasAssets = filesInFolder.Any(f => !f.EndsWith(".meta"));
            
            if (!hasAssets)
            {
                AssetDatabase.DeleteAsset(folder);
                Logger.Log($"Removed empty folder: {folder}");
                removedCount++;
            }
        }
        
        if (removedCount > 0)
        {
            AssetDatabase.Refresh();
            Logger.Log($"Removed {removedCount} empty folders");
        }
    }
    
    private static string GetUntrackedFolderForPrefab(string prefabName)
    {
        // Untracked 하위에 프리팹명 폴더 생성
        string untrackedPrefabFolder = $"{UNTRACKED_FOLDER}/{prefabName}";
        
        if (!AssetDatabase.IsValidFolder(untrackedPrefabFolder))
        {
            // Untracked 폴더가 없으면 먼저 생성
            CreateUntrackedFolder();
            AssetDatabase.CreateFolder(UNTRACKED_FOLDER, prefabName);
            Logger.Log($"Created folder: {untrackedPrefabFolder}");
        }
        
        return untrackedPrefabFolder;
    }
    
    private static bool MoveAssetToPrefabFolder(string originalPath, string prefabFolderPath)
    {
        if (!File.Exists(originalPath))
            return false;
            
        string fileName = Path.GetFileName(originalPath);
        string targetPath = $"{prefabFolderPath}/{fileName}";
        
        // Unity 경로 형식으로 정규화
        targetPath = targetPath.Replace("\\", "/");
        
        if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(targetPath) != null)
        {
            string nameWithoutExt = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);
            int counter = 1;
            
            do
            {
                string newFileName = $"{nameWithoutExt}_{counter}{extension}";
                targetPath = $"{prefabFolderPath}/{newFileName}";
                targetPath = targetPath.Replace("\\", "/");
                counter++;
            }
            while (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(targetPath) != null && counter < 1000);
            
            if (counter >= 1000)
            {
                Logger.Error($"Too many duplicate files for: {fileName}");
                return false;
            }
        }
        
        try
        {
            string result = AssetDatabase.MoveAsset(originalPath, targetPath);
            if (string.IsNullOrEmpty(result))
            {
                Logger.Log($"Moved: {originalPath} -> {targetPath}");
                return true;
            }
            else
            {
                Logger.Error($"Failed to move {originalPath}: {result}");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Exception moving {originalPath}: {e.Message}");
            return false;
        }
    }
}