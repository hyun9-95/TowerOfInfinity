using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Tools;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressableBuildGenerator : BaseGenerator
{
    private Dictionary<string, string> addressableDic = new Dictionary<string, string>();

    #region Exclude Path
    private HashSet<string> excludeNames = new HashSet<string>()
    {
        "NavMesh-NavigationSurface",
    };
    #endregion

    public void Generate(string addresableAssetPath)
    {
        UpdateEntry(addresableAssetPath);
        BuildAddressables();
    }

    public void UpdateEntry(string addresableAssetPath)
    {
        var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

        string[] guids = AssetDatabase.FindAssets("t:Object", new[] { addresableAssetPath });

        UpdateEntry(addressableSettings, guids);

        string json = JsonConvert.SerializeObject(addressableDic);
        SaveFileAtPath(addresableAssetPath + "/BuildInfo", NameDefine.AddressablePathName, json);
    }

    private void UpdateEntry(AddressableAssetSettings addressableSettings, string[] guids)
    {
        EditorUtility.SetDirty(addressableSettings);

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (AssetDatabase.IsValidFolder(assetPath))
                continue;

            string entryName = GetEntryName(assetPath);

            UpdateEntry(addressableSettings, guid, entryName, assetPath);
        }

        addressableSettings.OverridePlayerVersion = NameDefine.CurrentPlatformName;
        ClearNotUseEntries(addressableSettings, guids);
        RemoveEmptyGroups(addressableSettings);

        AssetDatabase.Refresh();
    }

    private string GetEntryName(string assetPath)
    {
        // 파츠별로 골라서 사용하기 때문에, 모든 파츠를 그룹 세분화해야 메모리가 덜 로드된다.
        if (assetPath.Contains("Assets/Addressable/CharacterBuilder/"))
        {
            string uniqueGroupName = assetPath.Replace("Assets/Addressable/", "");
            string[] split = uniqueGroupName.Split(".");
            uniqueGroupName = split[0];
            return uniqueGroupName;
        }

        string[] pathParts = assetPath.Split('/');
        string parentFolder = pathParts[2];

        // Core 폴더의 경우 하위 에셋 모두 하나의 그룹으로 묶기
        if (parentFolder.Contains("Core"))
            return parentFolder;

        if (pathParts.Length > 1)
        {
            string childFolder = pathParts[pathParts.Length - 2];

            if (childFolder.Equals(parentFolder))
                return childFolder;

            return $"{parentFolder}_{childFolder}";
        }

        return "Default";
    }

    private string GetLabelName(string assetPath)
    {
        // 일단은 Main만.. 도입부가 따로 없을 것 같아서
        return "Main";
    }

    private void UpdateEntry(AddressableAssetSettings addressableSettings, string guid, string entryName, string assetPath)
    {
        AddressableAssetEntry entry = addressableSettings.FindAssetEntry(guid);

        // 그룹 찾기
        AddressableAssetGroup targetGroup = addressableSettings.FindGroup(entryName);
        if (targetGroup == null)
        {
            var groupTemplate = addressableSettings.GetGroupTemplateObject(0) as AddressableAssetGroupTemplate;
            targetGroup = addressableSettings.CreateGroup(entryName, false, false, false, null, groupTemplate.GetTypes());
        }

        if (entry == null)
        {
            entry = addressableSettings.CreateOrMoveEntry(guid, targetGroup);
        }
        else if (entry.parentGroup != targetGroup)
        {
            addressableSettings.MoveEntry(entry, targetGroup);
        }

        if (entry == null)
            return;

        // 기존 라벨 제거함
        entry.labels.Clear();

        string labelName = GetLabelName(assetPath);

        // 라벨 추가
        if (!string.IsNullOrEmpty(labelName))
        {
            if (!addressableSettings.GetLabels().Contains(labelName))
                addressableSettings.AddLabel(labelName);

            entry.SetLabel(labelName, true);
        }

        string newAddress = assetPath.Replace("Assets/Addressable/", "");
        string[] split = newAddress.Split(".");
        newAddress = split[0];

        // 어드레스를 새로운 규칙에 따라 설정.
        entry.SetAddress(excludeNames.Contains(newAddress) ? assetPath : newAddress);
        AddToAddressableDic(newAddress, assetPath);
    }

    private AddressableAssetGroup FindDefaultGroup(AddressableAssetSettings addressableSettings)
    {
        AddressableAssetGroup defaultGroup = addressableSettings.FindGroup(NameDefine.AddressableDefaultGroupName);

        if (defaultGroup == null)
            defaultGroup = addressableSettings.FindGroup(NameDefine.AddressableDefaultGroupName_Newer);

        return defaultGroup;
    }

    private void AddToAddressableDic(string address, string path)
    {
        if (excludeNames.Contains(address))
            return;

        if (!addressableDic.ContainsKey(address))
        {
            addressableDic.Add(address, path);
            Logger.Log($"Add to AddressableDic {address} : {path}");
        }
        else
        {
            Logger.Error($"Duplicate path... {address}");
        }
    }

    private void ClearNotUseEntries(AddressableAssetSettings addressableSettings, string[] guids)
    {
        HashSet<string> guidsHashSet = new HashSet<string>(guids);

        foreach (AddressableAssetGroup group in addressableSettings.groups)
        {
            List<AddressableAssetEntry> removeList = new List<AddressableAssetEntry>();

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (!guidsHashSet.Contains(entry.guid))
                    removeList.Add(entry);
            }

            foreach (AddressableAssetEntry entryToRemove in removeList)
                group.RemoveAssetEntry(entryToRemove);
        }
    }

    private void RemoveEmptyGroups(AddressableAssetSettings addressableSettings)
    {
        List<AddressableAssetGroup> emptyGroups = new List<AddressableAssetGroup>();

        foreach (var group in addressableSettings.groups)
        {
            if (group == null || group.ReadOnly || group.entries.Count > 0)
                continue;

            if (group.Name.Contains("Default"))
                continue;

            emptyGroups.Add(group);
        }

        foreach (var group in emptyGroups)
        {
            Logger.Log($"Remove Empty Group: {group.Name}");
            addressableSettings.RemoveGroup(group);
        }
    }

    private void BuildAddressables()
    {
        string buildPath = GetLocalBuildPath();

        if (Directory.Exists(buildPath))
        {
            string[] files = Directory.GetFiles(buildPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AssetDatabase.Refresh();

        AddressableAssetSettings.BuildPlayerContent();
        AssetDatabase.Refresh();
    }

    public string GetLocalBuildPath()
    {
        string profileId = AddressableAssetSettingsDefaultObject.Settings.activeProfileId;
        var profileSettings = AddressableAssetSettingsDefaultObject.Settings.profileSettings;
        var localBuildPath = profileSettings.GetValueByName(profileId, "Local.BuildPath");

        string projectPath = Application.dataPath.Replace("/Assets", "");
        string buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
        localBuildPath = localBuildPath.Replace("[BuildTarget]", buildTarget);

        return $"{projectPath}/{localBuildPath}";
    }

    public static void UpdatePreviousAddressablesBuild()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        ContentUpdateScript.BuildContentUpdate(settings, PathDefine.AddressableBinPath);
    }

    public Dictionary<string, string> GenerateAddressableBuildInfo(string addressableAssetPath)
    {
        Dictionary<string, string> buildInfoDic = new Dictionary<string, string>();
        string[] guids = AssetDatabase.FindAssets("t:Object", new[] { addressableAssetPath });

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 폴더는 제외
            if (AssetDatabase.IsValidFolder(assetPath))
                continue;

            string assetAddress = assetPath.Replace("Assets/Addressable/", "");
            assetAddress = Path.GetFileNameWithoutExtension(assetAddress);

            // 중복 체크 및 추가
            if (!buildInfoDic.ContainsKey(assetAddress))
            {
                buildInfoDic.Add(assetAddress, assetPath);
                Logger.Log($"Add to BuildInfoDic {assetAddress} : {assetPath}");
            }
            else
            {
                Logger.Error($"Duplicate asset address found: {assetAddress}");
            }
        }

        return buildInfoDic;
    }

    public string GenerateAddressableBuildInfoJson(string addressableAssetPath)
    {
        var buildInfoDic = GenerateAddressableBuildInfo(addressableAssetPath);
        return JsonConvert.SerializeObject(buildInfoDic);
    }

    public void SaveAddressableBuildInfo(string addressableAssetPath)
    {
        string json = GenerateAddressableBuildInfoJson(addressableAssetPath);
        SaveFileAtPath(addressableAssetPath + "/BuildInfo", NameDefine.AddressablePathName, json);
    }
}
