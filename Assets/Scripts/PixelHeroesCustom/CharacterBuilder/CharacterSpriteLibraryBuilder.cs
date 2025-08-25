using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TowerOfInfinity.CharacterBuilder;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterSpriteLibraryBuilder : AddressableMono
{
    public bool IsInitialized => isInitialized;

    public enum Mode
    {
        OnDemand, // 필요한것만
        Preload,  // 전부 로드 (커마용)
    }

    public struct LibraryBuildInfo
    {
        public CharacterPartsType PartsType { get; }
        public string Address { get; }
        public string PartName { get; }
        public Texture2D Texture { get; set; }

        public LibraryBuildInfo(CharacterPartsType partsType, string address, string partsName, Texture2D texture = null)
        {
            PartsType = partsType;
            Address = address;
            PartName = partsName;
            Texture = texture;
        }

        public bool IsValid => !string.IsNullOrEmpty(Address);
        public bool IsLoaded => Texture != null;
    }

    public Mode CurrentMode { get; private set; } = Mode.OnDemand;
    
    private DataContainer<DataCharacterParts> partsContainer;
    private Texture2D mergedTexture;
    private Dictionary<string, Sprite> sprites;
    private Dictionary<CharacterPartsType, LibraryBuildInfo> loadedParts;
    private Dictionary<string, LibraryBuildInfo> preloadedParts;
    private HashSet<string> currentlyUsedAddresses;
    private Array partsEnumArray;
    private bool isPreloaded = false;
    private bool isInitialized = false;

    public void SetMode(Mode mode)
    {
        CurrentMode = mode;
    }

    public void Initialize()
    {
        partsEnumArray = Enum.GetValues(typeof(CharacterPartsType));
        partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
        loadedParts = new Dictionary<CharacterPartsType, LibraryBuildInfo>();
        preloadedParts = new Dictionary<string, LibraryBuildInfo>();
        currentlyUsedAddresses = new HashSet<string>();

        isInitialized = true;
    }

    public bool CheckPartsChange(MainCharacterPartsInfo newPartsInfo)
    {
        if (newPartsInfo?.PartsInfoDic == null)
            return true;

        // 현재 로드된 파츠가 없으면 변경된 것으로 간주
        if (loadedParts.Count == 0)
            return true;

        foreach (CharacterPartsType partType in partsEnumArray)
        {
            // 새로운 파츠 정보에서 해당 타입의 파츠 확인
            bool hasNewPart = newPartsInfo.PartsInfoDic.TryGetValue(partType, out var newCharacterPartsInfo);
            bool hasLoadedPart = loadedParts.TryGetValue(partType, out var loadedPartsInfo);

            // 한쪽에만 파츠가 있는 경우 변경됨
            if (hasNewPart != hasLoadedPart)
                return true;

            // 둘 다 파츠가 있는 경우 상세 비교
            if (hasNewPart && hasLoadedPart)
            {
                var newPartsData = newCharacterPartsInfo.GetPartsData();
                if (newPartsData == null)
                    return true;

                var newPartsName = newPartsData.PartsName;
                var newFinalPartsName = newCharacterPartsInfo.GetFormattedPartsName();
                var newAddress = CommonUtils.BuildPartsAddress(partType, newPartsName);

                // 주소가 다르거나 색상 정보가 다르면 변경됨
                if (loadedPartsInfo.Address != newAddress || 
                    loadedPartsInfo.PartName != newFinalPartsName)
                    return true;
            }
        }

        // 모든 파츠가 동일하면 변경 없음
        return false;
    }

    public async UniTask<SpriteLibraryAsset> CreateNewSpriteLibrary(Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        var libraryBuildInfos = BuildLibraryBuildInfosFromPartsInfo(partsInfoDic);
        await LoadTextures(libraryBuildInfos);

        UpdateCurrentlyUsedParts(libraryBuildInfos);

        var processedPartsPixels = ProcessAllPartsFromInfos(partsInfoDic, libraryBuildInfos);
        ApplyFirearmTransformationFromInfos(processedPartsPixels, partsInfoDic);

        var finalTexture = CreateMergedTexture(processedPartsPixels);
        var spriteLibraryAsset = CreateSpriteLibrary(finalTexture);

        ApplyCapeOverlayFromInfos(partsInfoDic, processedPartsPixels);

        return spriteLibraryAsset;
    }

    public async UniTask<SpriteLibraryAsset> CreateNewSpriteLibrary(string[] partsNames)
    {
        var libraryBuildInfos = BuildLibraryBuildInfos(partsNames);
        await LoadTextures(libraryBuildInfos);

        UpdateCurrentlyUsedParts(libraryBuildInfos);

        var processedPartsPixels = ProcessAllParts(libraryBuildInfos, partsNames);
        ApplyFirearmTransformation(processedPartsPixels, partsNames);

        var finalTexture = CreateMergedTexture(processedPartsPixels);
        var spriteLibraryAsset = CreateSpriteLibrary(finalTexture);

        ApplyCapeOverlay(partsNames, processedPartsPixels);

        return spriteLibraryAsset;
    }

    private string[] GetCurrentPartsNames()
    {
        var partsEnumArray = Enum.GetValues(typeof(CharacterPartsType));
        var currentParts = new string[partsEnumArray.Length];

        foreach (CharacterPartsType partType in partsEnumArray)
        {
            int index = (int)partType;

            if (loadedParts.TryGetValue(partType, out var partsInfo))
                currentParts[index] = partsInfo.PartName;
        }

        return currentParts;
    }

    private void UpdateCurrentlyUsedParts(List<LibraryBuildInfo> libraryBuildInfos)
    {
        currentlyUsedAddresses.Clear();
        
        foreach (var info in libraryBuildInfos)
        {
            if (info.IsValid)
                currentlyUsedAddresses.Add(info.Address);
        }
    }

    private List<LibraryBuildInfo> BuildLibraryBuildInfos(string[] partsNames)
    {
        var libraryBuildInfos = new List<LibraryBuildInfo>();

        foreach (CharacterPartsType partEnum in partsEnumArray)
        {
            int index = (int)partEnum;
            
            if (index >= partsNames.Length || string.IsNullOrEmpty(partsNames[index]))
                continue;

            string partsName = partsNames[index];
            string address = GetPartsAddressFromData(partEnum, partsName);
            
            var partsInfo = new LibraryBuildInfo(partEnum, address, partsName);
            
            if (partsInfo.IsValid)
                libraryBuildInfos.Add(partsInfo);
        }

        return libraryBuildInfos;
    }

    private List<LibraryBuildInfo> BuildLibraryBuildInfosFromPartsInfo(Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        var libraryBuildInfos = new List<LibraryBuildInfo>();
        
        if (partsInfoDic == null)
            return libraryBuildInfos;

        foreach (var kvp in partsInfoDic)
        {
            var partsInfo = kvp.Value;
            if (partsInfo == null || !partsInfo.IsValid())
                continue;

            var partsData = partsInfo.GetPartsData();
            if (partsData == null)
                continue;

            var finalPartsName = partsInfo.GetFormattedPartsName();
            if (string.IsNullOrEmpty(finalPartsName))
                continue;

            string address = GetPartsAddressFromData(partsData.PartsType, partsData.PartsName);
            var buildInfo = new LibraryBuildInfo(partsData.PartsType, address, finalPartsName);
            
            if (buildInfo.IsValid)
                libraryBuildInfos.Add(buildInfo);
        }

        return libraryBuildInfos;
    }

    private async UniTask LoadTextures(List<LibraryBuildInfo> libraryBuildInfos)
    {
        var loadTasks = new List<UniTask>();
        
        foreach (var info in libraryBuildInfos)
        {
            if (info.IsValid)
                loadTasks.Add(LoadAndStoreTexture(info.PartsType, info.Address));
        }
        
        await UniTask.WhenAll(loadTasks);
    }

    private Dictionary<CharacterPartsType, Color32[]> ProcessAllPartsFromInfos(Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic, List<LibraryBuildInfo> libraryBuildInfos)
    {
        var processedPartsPixels = new Dictionary<CharacterPartsType, Color32[]>();
        var libraryBuildInfoDic = libraryBuildInfos.ToDictionary(info => info.PartsType, info => info);

        ProcessSimpleParts(processedPartsPixels, libraryBuildInfoDic);
        ProcessBodyPartsFromInfos(processedPartsPixels, libraryBuildInfoDic, partsInfoDic);
        ProcessHeadPartsFromInfos(processedPartsPixels, libraryBuildInfoDic, partsInfoDic);
        ProcessArmorPartsFromInfos(processedPartsPixels, libraryBuildInfoDic, partsInfoDic);
        ProcessFacialPartsFromInfos(processedPartsPixels, libraryBuildInfoDic, partsInfoDic);
        ProcessAccessoryPartsFromInfos(processedPartsPixels, libraryBuildInfoDic, partsInfoDic);

        return processedPartsPixels;
    }

    private Dictionary<CharacterPartsType, Color32[]> ProcessAllParts(List<LibraryBuildInfo> partsInfos, string[] parts)
    {
        var processedPartsPixels = new Dictionary<CharacterPartsType, Color32[]>();
        var libraryBuildInfoDic = partsInfos.ToDictionary(info => info.PartsType, info => info);

        ProcessSimpleParts(processedPartsPixels, libraryBuildInfoDic);
        ProcessBodyParts(processedPartsPixels, libraryBuildInfoDic, parts);
        ProcessHeadParts(processedPartsPixels, libraryBuildInfoDic, parts);
        ProcessArmorParts(processedPartsPixels, libraryBuildInfoDic, parts);
        ProcessFacialParts(processedPartsPixels, libraryBuildInfoDic, parts);
        ProcessAccessoryParts(processedPartsPixels, libraryBuildInfoDic, parts);

        return processedPartsPixels;
    }

    private void ProcessSimpleParts(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic)
    {
        var simpleParts = new[] { CharacterPartsType.Back, CharacterPartsType.Shield };
        
        foreach (var partType in simpleParts)
        {
            if (libraryBuildInfoDic.ContainsKey(partType))
                processedPixels.Add(partType, ProcessPixels(partType, libraryBuildInfoDic[partType].PartName));
        }
    }

    private void ProcessBodyPartsFromInfos(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        if (!libraryBuildInfoDic.ContainsKey(CharacterPartsType.Body))
            return;

        var bodyInfo = libraryBuildInfoDic[CharacterPartsType.Body];
        processedPixels.Add(CharacterPartsType.Body, ProcessPixels(CharacterPartsType.Body, bodyInfo.PartName));

        bool hasFirearm = partsInfoDic.ContainsKey(CharacterPartsType.Firearm);
        if (!hasFirearm && libraryBuildInfoDic.ContainsKey(CharacterPartsType.Arms))
            processedPixels.Add(CharacterPartsType.Arms, ProcessPixels(CharacterPartsType.Arms, bodyInfo.PartName));
    }

    private void ProcessBodyParts(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, string[] parts)
    {
        if (!libraryBuildInfoDic.ContainsKey(CharacterPartsType.Body))
            return;

        var bodyInfo = libraryBuildInfoDic[CharacterPartsType.Body];
        processedPixels.Add(CharacterPartsType.Body, ProcessPixels(CharacterPartsType.Body, bodyInfo.PartName));

        if (string.IsNullOrEmpty(parts[(int)CharacterPartsType.Firearm]) && libraryBuildInfoDic.ContainsKey(CharacterPartsType.Arms))
            processedPixels.Add(CharacterPartsType.Arms, ProcessPixels(CharacterPartsType.Arms, bodyInfo.PartName));
    }

    private void ProcessHeadPartsFromInfos(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Head))
            processedPixels.Add(CharacterPartsType.Head, ProcessPixels(CharacterPartsType.Head, libraryBuildInfoDic[CharacterPartsType.Head].PartName));

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Ears))
        {
            bool hasHelmet = partsInfoDic.ContainsKey(CharacterPartsType.Helmet);
            bool showEars = !hasHelmet;
            
            if (hasHelmet)
            {
                var helmetInfo = partsInfoDic[CharacterPartsType.Helmet];
                var helmetName = helmetInfo.GetFormattedPartsName();
                showEars = !string.IsNullOrEmpty(helmetName) && helmetName.Contains("ShowEars");
            }
            
            if (showEars)
                processedPixels.Add(CharacterPartsType.Ears, ProcessPixels(CharacterPartsType.Ears, libraryBuildInfoDic[CharacterPartsType.Ears].PartName));
        }
    }

    private void ProcessHeadParts(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, string[] parts)
    {
        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Head))
            processedPixels.Add(CharacterPartsType.Head, ProcessPixels(CharacterPartsType.Head, libraryBuildInfoDic[CharacterPartsType.Head].PartName));

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Ears))
        {
            var helmetPart = parts[(int)CharacterPartsType.Helmet];
            if (string.IsNullOrEmpty(helmetPart) || helmetPart.Contains("ShowEars"))
                processedPixels.Add(CharacterPartsType.Ears, ProcessPixels(CharacterPartsType.Ears, libraryBuildInfoDic[CharacterPartsType.Ears].PartName));
        }
    }

    private void ProcessArmorPartsFromInfos(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        if (!libraryBuildInfoDic.ContainsKey(CharacterPartsType.Armor))
            return;

        var armorInfo = libraryBuildInfoDic[CharacterPartsType.Armor];
        processedPixels.Add(CharacterPartsType.Armor, ProcessPixels(CharacterPartsType.Armor, armorInfo.PartName));

        bool hasFirearm = partsInfoDic.ContainsKey(CharacterPartsType.Firearm);
        if (!hasFirearm && libraryBuildInfoDic.ContainsKey(CharacterPartsType.Bracers))
            processedPixels.Add(CharacterPartsType.Bracers, ProcessPixels(CharacterPartsType.Bracers, armorInfo.PartName));
    }

    private void ProcessArmorParts(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, string[] parts)
    {
        if (!libraryBuildInfoDic.ContainsKey(CharacterPartsType.Armor))
            return;

        var armorInfo = libraryBuildInfoDic[CharacterPartsType.Armor];
        processedPixels.Add(CharacterPartsType.Armor, ProcessPixels(CharacterPartsType.Armor, armorInfo.PartName));

        if (string.IsNullOrEmpty(parts[(int)CharacterPartsType.Firearm]) && libraryBuildInfoDic.ContainsKey(CharacterPartsType.Bracers))
            processedPixels.Add(CharacterPartsType.Bracers, ProcessPixels(CharacterPartsType.Bracers, armorInfo.PartName));
    }

    private void ProcessFacialPartsFromInfos(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Eyes))
            processedPixels.Add(CharacterPartsType.Eyes, ProcessPixels(CharacterPartsType.Eyes, libraryBuildInfoDic[CharacterPartsType.Eyes].PartName));

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Hair))
        {
            bool hasHelmet = partsInfoDic.ContainsKey(CharacterPartsType.Helmet);
            Color32[] hairMask = (!hasHelmet || !processedPixels.ContainsKey(CharacterPartsType.Head)) ? null : processedPixels[CharacterPartsType.Head];
            processedPixels.Add(CharacterPartsType.Hair, ProcessPixels(CharacterPartsType.Hair, libraryBuildInfoDic[CharacterPartsType.Hair].PartName, hairMask));
        }

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Mask))
            processedPixels.Add(CharacterPartsType.Mask, ProcessPixels(CharacterPartsType.Mask, libraryBuildInfoDic[CharacterPartsType.Mask].PartName));
    }

    private void ProcessFacialParts(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, string[] parts)
    {
        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Eyes))
            processedPixels.Add(CharacterPartsType.Eyes, ProcessPixels(CharacterPartsType.Eyes, libraryBuildInfoDic[CharacterPartsType.Eyes].PartName));

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Hair))
        {
            var helmetPart = parts[(int)CharacterPartsType.Helmet];
            Color32[] hairMask = (string.IsNullOrEmpty(helmetPart) || !processedPixels.ContainsKey(CharacterPartsType.Head)) ? null : processedPixels[CharacterPartsType.Head];
            processedPixels.Add(CharacterPartsType.Hair, ProcessPixels(CharacterPartsType.Hair, libraryBuildInfoDic[CharacterPartsType.Hair].PartName, hairMask));
        }

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Mask))
            processedPixels.Add(CharacterPartsType.Mask, ProcessPixels(CharacterPartsType.Mask, libraryBuildInfoDic[CharacterPartsType.Mask].PartName));
    }

    private void ProcessAccessoryPartsFromInfos(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        var accessoryParts = new[] { CharacterPartsType.Cape, CharacterPartsType.Helmet, CharacterPartsType.Weapon };
        
        foreach (var partType in accessoryParts)
        {
            if (libraryBuildInfoDic.ContainsKey(partType))
                processedPixels.Add(partType, ProcessPixels(partType, libraryBuildInfoDic[partType].PartName));
        }

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Horns) && !partsInfoDic.ContainsKey(CharacterPartsType.Horns))
            processedPixels.Add(CharacterPartsType.Horns, ProcessPixels(CharacterPartsType.Horns, libraryBuildInfoDic[CharacterPartsType.Horns].PartName));
    }

    private void ProcessAccessoryParts(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, LibraryBuildInfo> libraryBuildInfoDic, string[] parts)
    {
        var accessoryParts = new[] { CharacterPartsType.Cape, CharacterPartsType.Helmet, CharacterPartsType.Weapon };
        
        foreach (var partType in accessoryParts)
        {
            if (libraryBuildInfoDic.ContainsKey(partType))
                processedPixels.Add(partType, ProcessPixels(partType, libraryBuildInfoDic[partType].PartName));
        }

        if (libraryBuildInfoDic.ContainsKey(CharacterPartsType.Horns) && string.IsNullOrEmpty(parts[(int)CharacterPartsType.Horns]))
            processedPixels.Add(CharacterPartsType.Horns, ProcessPixels(CharacterPartsType.Horns, libraryBuildInfoDic[CharacterPartsType.Horns].PartName));
    }

    private Color32[] ProcessPixels(CharacterPartsType partsType, string partData, Color32[] mask = null)
    {
        if (!loadedParts.TryGetValue(partsType, out LibraryBuildInfo partsInfo) || !partsInfo.IsLoaded)
            return null;

        return ProcessPartsPixels(partsInfo.PartsType.ToString(), partsInfo.Texture, partData, mask);
    }

    private void ApplyFirearmTransformationFromInfos(Dictionary<CharacterPartsType, Color32[]> processedPixels, Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic)
    {
        if (!partsInfoDic.ContainsKey(CharacterPartsType.Firearm))
            return;

        const int width = 576;
        var partsToTransform = new[] { CharacterPartsType.Head, CharacterPartsType.Ears, CharacterPartsType.Eyes, CharacterPartsType.Mask, CharacterPartsType.Hair, CharacterPartsType.Helmet };

        foreach (var partType in partsToTransform)
        {
            if (!processedPixels.ContainsKey(partType) || processedPixels[partType] == null)
                continue;

            var copy = processedPixels[partType].ToArray();

            for (var y = 11 * 64 - 1; y >= 10 * 64 - 1; y--)
            {
                for (var x = 0; x < width; x++)
                    copy[x + y * width] = copy[x + (y - 1) * width];
            }
            
            processedPixels[partType] = copy;
        }
    }

    private void ApplyFirearmTransformation(Dictionary<CharacterPartsType, Color32[]> processedPixels, string[] parts)
    {
        if (string.IsNullOrEmpty(parts[(int)CharacterPartsType.Firearm]))
            return;

        const int width = 576;
        var partsToTransform = new[] { CharacterPartsType.Head, CharacterPartsType.Ears, CharacterPartsType.Eyes, CharacterPartsType.Mask, CharacterPartsType.Hair, CharacterPartsType.Helmet };

        foreach (var partType in partsToTransform)
        {
            if (!processedPixels.ContainsKey(partType) || processedPixels[partType] == null)
                continue;

            var copy = processedPixels[partType].ToArray();

            for (var y = 11 * 64 - 1; y >= 10 * 64 - 1; y--)
            {
                for (var x = 0; x < width; x++)
                    copy[x + y * width] = copy[x + (y - 1) * width];
            }
            
            processedPixels[partType] = copy;
        }
    }

    private Texture2D CreateMergedTexture(Dictionary<CharacterPartsType, Color32[]> processedPixels)
    {
        const int width = 576;
        const int height = 928;

        var fixedPartOrder = new[] { CharacterPartsType.Back, CharacterPartsType.Shield, CharacterPartsType.Body, CharacterPartsType.Arms, CharacterPartsType.Head, CharacterPartsType.Ears, CharacterPartsType.Armor, CharacterPartsType.Bracers, CharacterPartsType.Eyes, CharacterPartsType.Hair, CharacterPartsType.Cape, CharacterPartsType.Helmet, CharacterPartsType.Weapon, CharacterPartsType.Firearm, CharacterPartsType.Mask, CharacterPartsType.Horns };
        var finalOrderedPixels = new List<Color32[]>();

        foreach (var partType in fixedPartOrder)
        {
            if (processedPixels.ContainsKey(partType) && processedPixels[partType] != null)
                finalOrderedPixels.Add(processedPixels[partType]);
        }

        if (mergedTexture == null)
            mergedTexture = new Texture2D(width, height) { filterMode = FilterMode.Point };

        if (finalOrderedPixels.Count == 0)
        {
            var emptyPixels = new Color32[width * height];
            for (int i = 0; i < emptyPixels.Length; i++)
                emptyPixels[i] = new Color32(0, 0, 0, 0);
            
            mergedTexture.SetPixels32(emptyPixels);
        }
        else
        {
            mergedTexture = TextureProcessor.MergeLayers(mergedTexture, finalOrderedPixels.ToArray());
        }
        
        mergedTexture.Apply();
        return mergedTexture;
    }

    private SpriteLibraryAsset CreateSpriteLibrary(Texture2D texture)
    {
        if (sprites == null)
        {
            var clipNames = new List<string> { "Idle", "Ready", "Run", "Crawl", "Climb", "Jump", "Push", "Jab", "Slash", "Shot", "Fire", "Block", "Death", "Roll" };
            clipNames.Reverse();
            sprites = new Dictionary<string, Sprite>();

            for (var i = 0; i < clipNames.Count; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    var key = clipNames[i] + "_" + j;
                    sprites.Add(key, Sprite.Create(texture, new Rect(j * 64, i * 64, 64, 64), new Vector2(0.5f, 0.125f), 16, 0, SpriteMeshType.FullRect));
                }
            }
        }

        var spriteLibraryAsset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();

        foreach (var sprite in sprites)
        {
            var split = sprite.Key.Split("_");
            spriteLibraryAsset.AddCategoryLabel(sprite.Value, split[0], split[1]);
        }

        return spriteLibraryAsset;
    }

    private void ApplyCapeOverlayFromInfos(Dictionary<CharacterPartsType, CharacterPartsInfo> partsInfoDic, Dictionary<CharacterPartsType, Color32[]> processedPixels)
    {
        if (partsInfoDic.ContainsKey(CharacterPartsType.Cape) && processedPixels.ContainsKey(CharacterPartsType.Cape))
            CapeOverlay(processedPixels[CharacterPartsType.Cape]);
    }

    private void ApplyCapeOverlay(string[] parts, Dictionary<CharacterPartsType, Color32[]> processedPixels)
    {
        if (!string.IsNullOrEmpty(parts[(int)CharacterPartsType.Cape]) && processedPixels.ContainsKey(CharacterPartsType.Cape))
            CapeOverlay(processedPixels[CharacterPartsType.Cape]);
    }

    private async UniTask LoadAndStoreTexture(CharacterPartsType partsType, string texturePath)
    {
        if (CurrentMode == Mode.Preload && isPreloaded)
        {
            if (TryUsePreloadedTexture(partsType, texturePath))
                return;
        }
        
        if (IsTextureAlreadyLoaded(partsType, texturePath))
            return;

        ReleaseExistingTexture(partsType);

        var texture = await AddressableManager.Instance.LoadAssetAsyncWithTracker<Texture2D>(texturePath, this);

        if (texture == null)
        {
            Logger.Error($"Character Parts Load Failed : {texturePath}");
            return;
        }

        var libraryBuildInfo = new LibraryBuildInfo(partsType, texturePath, string.Empty, texture);
        loadedParts[partsType] = libraryBuildInfo;
    }

    private bool TryUsePreloadedTexture(CharacterPartsType partsType, string texturePath)
    {
        if (!preloadedParts.TryGetValue(texturePath, out var preloadedInfo))
            return false;
            
        if (preloadedInfo.IsLoaded && preloadedInfo.PartsType == partsType)
        {
            var libraryBuildInfo = new LibraryBuildInfo(partsType, texturePath, preloadedInfo.PartName, preloadedInfo.Texture);
            loadedParts[partsType] = libraryBuildInfo;
            return true;
        }
            
        return false;
    }

    private bool IsTextureAlreadyLoaded(CharacterPartsType partsType, string texturePath)
    {
        if (!loadedParts.TryGetValue(partsType, out var libraryBuildInfo))
            return false;

        return libraryBuildInfo.Address.Equals(texturePath);
    }

    private void ReleaseExistingTexture(CharacterPartsType partsType)
    {
        if (CurrentMode != Mode.OnDemand)
            return;
            
        if (!loadedParts.TryGetValue(partsType, out var libraryBuildInfo) || !libraryBuildInfo.IsLoaded)
            return;

        AddressableManager.Instance.ReleaseFromTracker(libraryBuildInfo.Texture, gameObject);
    }

    private string GetPartsAddressFromData(CharacterPartsType partsType, string partName)
    {
        if (partsContainer == null)
            partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
            
        if (partsContainer == null) 
            return null;

        var targetPart = partsContainer.Find(p => 
            p.PartsType == partsType && 
            p.PartsName == partName);
            
        return targetPart.IsNullOrEmpty() ? null : CommonUtils.BuildPartsAddress(partsType, targetPart.PartsName);
    }
    
    private string GetPartNameFromPath(string partsPath)
    {
        if (string.IsNullOrEmpty(partsPath))
            return string.Empty;
            
        var lastSlashIndex = partsPath.LastIndexOf('/');
        
        if (lastSlashIndex < 0 || lastSlashIndex >= partsPath.Length - 1)
            return partsPath;
            
        return partsPath.Substring(lastSlashIndex + 1);
    }

    private Color32[] ProcessPartsPixels(string partsName, Texture2D sourceTexture, string data, Color32[] mask)
    {
        if (sourceTexture == null) 
            return null;

        var (paint, h, s, v) = ParsePixelProcessingData(data);
        Color32[] pixels = sourceTexture.GetPixels32();

        ApplyMask(pixels, mask);
        pixels = ApplyPaint(pixels, paint, partsName);
        ApplyColorAdjustment(pixels, h, s, v);
        
        return TextureProcessor.ApplyPalette(pixels, TextureProcessor.Palette);
    }

    private (Color paint, float h, float s, float v) ParsePixelProcessingData(string data)
    {
        var match = Regex.Match(data, @"(?<Name>[\w\- \[\]]+)(?<Paint>#\w+)?(?:/(?<H>[-\d]+):(?<S>[-\d]+):(?<V>[-\d]+))?");

        Color paint = Color.white;
        if (match.Groups["Paint"].Success)
            ColorUtility.TryParseHtmlString(match.Groups["Paint"].Value, out paint);

        float h = 0f, s = 0f, v = 0f;
        if (match.Groups["H"].Success && match.Groups["S"].Success && match.Groups["V"].Success)
        {
            h = float.Parse(match.Groups["H"].Value, CultureInfo.InvariantCulture);
            s = float.Parse(match.Groups["S"].Value, CultureInfo.InvariantCulture);
            v = float.Parse(match.Groups["V"].Value, CultureInfo.InvariantCulture);
        }

        return (paint, h, s, v);
    }

    private void ApplyMask(Color32[] pixels, Color32[] mask)
    {
        if (mask == null)
            return;

        for (var i = 0; i < pixels.Length; i++)
        {
            if (mask[i].a <= 0)
                pixels[i] = new Color32();
            else if (mask[i] == Color.black)
                pixels[i] = mask[i];
        }
    }

    private Color32[] ApplyPaint(Color32[] pixels, Color paint, string partsName)
    {
        if (paint == Color.white)
            return pixels;

        if (IsBodyParts(partsName))
            return TextureProcessor.Repaint3C(pixels, paint, TextureProcessor.Palette);

        for (var i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0) 
                pixels[i] = (Color32)((Color)pixels[i] * paint);
        }
        
        return pixels;
    }

    private bool IsBodyParts(string partsName)
    {
        return partsName == "Head" || partsName == "Body" || partsName == "Arms" || partsName == "Hair";
    }

    private void ApplyColorAdjustment(Color32[] pixels, float h, float s, float v)
    {
        if (Mathf.Approximately(h, 0) && Mathf.Approximately(s, 0) && Mathf.Approximately(v, 0))
            return;

        for (var i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0 && pixels[i] != Color.black)
                pixels[i] = TextureProcessor.AdjustColor(pixels[i], h, s, v);
        }
    }

    private void CapeOverlay(Color32[] cape)
    {
        var pixels = mergedTexture.GetPixels32();
        var width = mergedTexture.width;
        var height = mergedTexture.height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (x >= 0 && x < 2 * 64 && y >= 9 * 64 && y < 10 * 64
                    || x >= 64 && x < 64 + 2 * 64 && y >= 6 * 64 && y < 7 * 64
                    || x >= 128 && x < 128 + 2 * 64 && y >= 5 * 64 && y < 6 * 64
                    || x >= 0 && x < 4 * 64 && y >= 4 * 64 && y < 5 * 64)
                {
                    var i = x + y * width;

                    if (cape[i].a > 0) pixels[i] = cape[i];
                }
            }
        }

        mergedTexture.SetPixels32(pixels);
        mergedTexture.Apply();
    }

    private static Vector2 GetMuzzlePosition(Texture2D texture)
    {
        var muzzlePosition = new Vector2(texture.width / 2f - 1, 6);

        for (var x = 63; x >= 0; x--)
        {
            for (var y = 0; y < 64; y++)
            {
                if (texture.GetPixel(x, y).a > 0)
                {
                    return muzzlePosition;
                }
            }

            muzzlePosition.x = x - 1 - texture.width / 2f;
        }

        return muzzlePosition;
    }

    public async UniTask Preload()
    {
        if (CurrentMode != Mode.Preload)
            return;

        if (partsContainer == null)
            partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
        
        var preloadParts = partsContainer.FindAll(x => !x.IsEmpty && 
            (x.Category == CharacterPartsCategory.Race || x.Category == CharacterPartsCategory.Hair));
        
        var preloadTasks = CreatePreloadTasks(preloadParts);
        
        await UniTask.WhenAll(preloadTasks);
        isPreloaded = true;
        
        Logger.Log($"Preload completed: {preloadedParts.Count} parts loaded");
    }

    private List<UniTask> CreatePreloadTasks(IEnumerable<DataCharacterParts> preloadParts)
    {
        var preloadTasks = new List<UniTask>();
        
        foreach (var part in preloadParts)
        {
            if (string.IsNullOrEmpty(part.PartsName))
                continue;

            preloadTasks.Add(PreloadTexture(part.PartsType, part.PartsName, CommonUtils.BuildPartsAddress(part.PartsType, part.PartsName)));
        }
        
        return preloadTasks;
    }
    
    private async UniTask PreloadTexture(CharacterPartsType partsType, string partName, string address)
    {
        if (preloadedParts.ContainsKey(address))
            return;

        var texture = await AddressableManager.Instance.LoadAssetAsyncWithTracker<Texture2D>(address, this);

        if (texture == null)
        {
            Logger.Error($"Failed to preload texture: {address}");
            return;
        }

        var partsInfo = new LibraryBuildInfo(partsType, address, partName, texture);
        preloadedParts[address] = partsInfo;
    }
    
    
    public void ResetPreload()
    {
        if (CurrentMode != Mode.Preload)
            return;
        
        var addressesToRelease = new List<string>();
        
        foreach (var kvp in preloadedParts)
        {
            var address = kvp.Key;
            bool isCurrentlyInUse = IsAddressCurrentlyInUse(address);
            
            if (!isCurrentlyInUse)
                addressesToRelease.Add(address);
        }
        
        foreach (var address in addressesToRelease)
        {
            if (preloadedParts.TryGetValue(address, out var partsInfo) && partsInfo.IsLoaded)
            {
                AddressableManager.Instance.ReleaseFromTracker(partsInfo.Texture, gameObject);
                preloadedParts.Remove(address);
            }
        }
        
        Logger.Log($"Preload reset: {addressesToRelease.Count} parts released.");
    }

    private bool IsAddressCurrentlyInUse(string address)
    {
        return currentlyUsedAddresses.Contains(address);
    }
    
    public override void OnDestroy()
    {
        foreach (var partsInfo in loadedParts.Values)
        {
            if (partsInfo.IsLoaded)
                AddressableManager.Instance.ReleaseFromTracker(partsInfo.Texture, gameObject);
        }

        foreach (var partsInfo in preloadedParts.Values)
        {
            if (partsInfo.IsLoaded)
                AddressableManager.Instance.ReleaseFromTracker(partsInfo.Texture, gameObject);
        }

        AddressableManager.Instance.ReleaseGameObject(gameObject);
    }
}