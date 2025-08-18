using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Tools;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
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

    #region PackSeparately Path
    private HashSet<string> packSeparatelyPaths = new HashSet<string>()
    {
        "CharacterPartsIcon",
        "CharacterBuilder",
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
        SetPackSeparatelyForGroups(addressableSettings);

        AssetDatabase.Refresh();
    }

    private string GetEntryName(string assetPath)
    {
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
            if (entry != null)
                Logger.Log($"New addressable entry created: {assetPath}");
        }
        else if (entry.parentGroup != targetGroup)
        {
            addressableSettings.MoveEntry(entry, targetGroup);
            Logger.Log($"Addressable entry moved to group '{entryName}': {assetPath}");
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

        string newAddress = null;

        if (assetPath.Contains("Scenes"))
        {
            newAddress = Path.GetFileNameWithoutExtension(assetPath);
        }
        else
        {
            newAddress = assetPath.Replace("Assets/Addressable/", "");
            string[] split = newAddress.Split(".");
            newAddress = split[0];
        }

        entry.SetAddress(excludeNames.Contains(newAddress) ? assetPath : newAddress);
        AddToAddressableDic(newAddress, assetPath);
    }

    private void SetPackSeparatelyForGroups(AddressableAssetSettings addressableSettings)
    {
        foreach (var group in addressableSettings.groups)
        {
            if (group == null || group.ReadOnly)
                continue;

            bool shouldPackSeparately = false;
            foreach (string path in packSeparatelyPaths)
            {
                if (group.Name.Contains(path))
                {
                    shouldPackSeparately = true;
                    break;
                }
            }

            if (shouldPackSeparately)
            {
                var bundledSchema = group.GetSchema<BundledAssetGroupSchema>();
                if (bundledSchema != null)
                {
                    bundledSchema.BundleMode = 
                        BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                }
            }
        }
    }

    private void AddToAddressableDic(string address, string path)
    {
        if (excludeNames.Contains(address))
            return;

        if (!addressableDic.ContainsKey(address))
        {
            addressableDic.Add(address, path);
        }
        else
        {
            Logger.Error($"Duplicate path... {address}");
        }
    }

    private void ClearNotUseEntries(AddressableAssetSettings addressableSettings, string[] guids)
    {
        HashSet<string> guidsHashSet = new HashSet<string>(guids);
        List<AddressableAssetGroup> removeGroupList = new List<AddressableAssetGroup>();

        foreach (AddressableAssetGroup group in addressableSettings.groups)
        {
            List<AddressableAssetEntry> removeList = new List<AddressableAssetEntry>();

            if (group == null)
            {
                removeGroupList.Add(group);
                continue;
            }

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (!guidsHashSet.Contains(entry.guid))
                    removeList.Add(entry);
            }

            foreach (AddressableAssetEntry entryToRemove in removeList)
                group.RemoveAssetEntry(entryToRemove);
        }

        foreach (var group in removeGroupList)
            addressableSettings.RemoveGroup(group);
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
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
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
}
