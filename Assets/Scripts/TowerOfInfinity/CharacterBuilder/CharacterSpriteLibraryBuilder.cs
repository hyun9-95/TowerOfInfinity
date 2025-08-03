using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TowerOfInfinity.CharacterBuilder;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterSpriteLibraryBuilder : MonoBehaviour
{
    public enum Mode
    {
        OnDemand, // 필요한것만
        Preload,  // 전부 로드 (커마용)
    }

    public Mode CurrentMode { get; private set; } = Mode.OnDemand;
    
    private DataContainer<DataCharacterParts> partsContainer;
    private Texture2D mergedTexture;
    private Dictionary<string, Sprite> sprites;
    private Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();
    private Dictionary<string, string> loadedAddress = new Dictionary<string, string>();
    private Dictionary<string, Texture2D> preloadedTextures = new Dictionary<string, Texture2D>();
    private Dictionary<string, string> preloadedAddress = new Dictionary<string, string>();
    private bool isPreloaded = false;

    public void SetMode(Mode mode)
    {
        CurrentMode = mode;
    }

    public async UniTask<SpriteLibraryAsset> Rebuild(MainCharacterPartsInfo userCharacterPartsInfo)
    {
        var partsEnumArray = Enum.GetValues(typeof(CharacterPartsType));
        var parts = new string[partsEnumArray.Length];

        if (userCharacterPartsInfo?.PartsInfoDic != null)
        {
            foreach (CharacterPartsType partType in partsEnumArray)
            {
                int index = (int)partType;
                if (userCharacterPartsInfo.PartsInfoDic.TryGetValue(partType, out var partsData))
                {
                    var partsPath = partsData.PartsPath;
                    var colorCode = partsData.ColorCode;
                    
                    if (!string.IsNullOrEmpty(partsPath))
                    {
                        parts[index] = string.IsNullOrEmpty(colorCode) ? partsPath : $"{partsPath}{colorCode}";
                    }
                }
            }
        }

        return await Rebuild(parts);
    }

    public async UniTask<SpriteLibraryAsset> Rebuild(string[] parts)
    {
        var partsEnumArray = Enum.GetValues(typeof(CharacterPartsType));

        if (partsEnumArray.Length != parts.Length)
            return null;

        if (partsContainer == null)
            partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();

        var width = 576;
        var height = 928;

        var layersToLoad = new Dictionary<string, string>();
        var layerPartData = new Dictionary<string, string>();

        foreach (CharacterPartsType partEnum in partsEnumArray)
        {
            int index = (int)partEnum;
            if (index < parts.Length && !string.IsNullOrEmpty(parts[index]))
            {
                string partName = partEnum.ToString();
                string partValue = parts[index];
                string address = GetAddressFromAddressableData(partName, partValue);
                
                if (!string.IsNullOrEmpty(address))
                {
                    layersToLoad[partName] = address;
                    layerPartData[partName] = partValue;
                }
            }
        }

        List<UniTask> loadTasks = new List<UniTask>();
        foreach (var entry in layersToLoad)
        {
            string layerName = entry.Key;
            string address = entry.Value;

            if (!string.IsNullOrEmpty(address))
            {
                loadTasks.Add(LoadAndStoreTexture(layerName, address));
            }
        }
        await UniTask.WhenAll(loadTasks);

        var processedLayersPixels = new Dictionary<string, Color32[]>();

        Func<string, string, Color32[], Color32[]> getProcessedPixels = (layer, partData, mask) =>
        {
            if (loadedTextures.TryGetValue(layer, out Texture2D texture))
            {
                return ProcessLayerPixels(layer, texture, partData, mask);
            }
            return null;
        };

        if (layerPartData.ContainsKey("Back")) processedLayersPixels.Add("Back", getProcessedPixels("Back", layerPartData["Back"], null));
        if (layerPartData.ContainsKey("Shield")) processedLayersPixels.Add("Shield", getProcessedPixels("Shield", layerPartData["Shield"], null));

        if (layerPartData.ContainsKey("Body"))
        {
            processedLayersPixels.Add("Body", getProcessedPixels("Body", layerPartData["Body"], null));

            if (string.IsNullOrEmpty(parts[(int)CharacterPartsType.Firearm]) && layerPartData.ContainsKey("Arms"))
            {
                processedLayersPixels.Add("Arms", getProcessedPixels("Arms", layerPartData["Body"], null));
            }
        }

        if (layerPartData.ContainsKey("Head")) processedLayersPixels.Add("Head", getProcessedPixels("Head", layerPartData["Head"], null));
        if (layerPartData.ContainsKey("Ears") && (string.IsNullOrEmpty(parts[(int)CharacterPartsType.Helmet]) || parts[(int)CharacterPartsType.Helmet].Contains("ShowEars")))
        {
            processedLayersPixels.Add("Ears", getProcessedPixels("Ears", layerPartData["Ears"], null));
        }

        if (layerPartData.ContainsKey("Armor"))
        {
            processedLayersPixels.Add("Armor", getProcessedPixels("Armor", layerPartData["Armor"], null));

            if (string.IsNullOrEmpty(parts[(int)CharacterPartsType.Firearm]) && layerPartData.ContainsKey("Bracers"))
            {
                processedLayersPixels.Add("Bracers", getProcessedPixels("Bracers", layerPartData["Armor"], null));
            }
        }

        if (layerPartData.ContainsKey("Eyes")) processedLayersPixels.Add("Eyes", getProcessedPixels("Eyes", layerPartData["Eyes"], null));
        if (layerPartData.ContainsKey("Hair"))
        {
            Color32[] hairMask = (string.IsNullOrEmpty(parts[(int)CharacterPartsType.Helmet]) || !processedLayersPixels.ContainsKey("Head")) ? null : processedLayersPixels["Head"];
            processedLayersPixels.Add("Hair", getProcessedPixels("Hair", layerPartData["Hair"], hairMask));
        }
        if (layerPartData.ContainsKey("Cape")) processedLayersPixels.Add("Cape", getProcessedPixels("Cape", layerPartData["Cape"], null));
        if (layerPartData.ContainsKey("Helmet")) processedLayersPixels.Add("Helmet", getProcessedPixels("Helmet", layerPartData["Helmet"], null));
        if (layerPartData.ContainsKey("Weapon")) processedLayersPixels.Add("Weapon", getProcessedPixels("Weapon", layerPartData["Weapon"], null));

        if (layerPartData.ContainsKey("Mask")) processedLayersPixels.Add("Mask", getProcessedPixels("Mask", layerPartData["Mask"], null));
        if (layerPartData.ContainsKey("Horns") && string.IsNullOrEmpty(parts[(int)CharacterPartsType.Horns])) processedLayersPixels.Add("Horns", getProcessedPixels("Horns", layerPartData["Horns"], null));

        if (!string.IsNullOrEmpty(parts[(int)CharacterPartsType.Firearm]))
        {
            foreach (var layerName in new[] { "Head", "Ears", "Eyes", "Mask", "Hair", "Helmet" })
            {
                if (processedLayersPixels.ContainsKey(layerName) && processedLayersPixels[layerName] != null)
                {
                    var copy = processedLayersPixels[layerName].ToArray();

                    for (var y = 11 * 64 - 1; y >= 10 * 64 - 1; y--)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            copy[x + y * width] = copy[x + (y - 1) * width];
                        }
                    }
                    processedLayersPixels[layerName] = copy;
                }
            }
        }

        var fixedLayerOrder = new List<string>
            {
                "Back", "Shield", "Body", "Arms", "Head", "Ears", "Armor", "Bracers", "Eyes", "Hair", "Cape", "Helmet", "Weapon", "Firearm", "Mask", "Horns"
            };

        var finalOrderedPixels = new List<Color32[]>();
        foreach (var layerName in fixedLayerOrder)
        {
            if (processedLayersPixels.ContainsKey(layerName) && processedLayersPixels[layerName] != null)
            {
                finalOrderedPixels.Add(processedLayersPixels[layerName]);
            }
        }

        if (mergedTexture == null)
            mergedTexture = new Texture2D(width, height) { filterMode = FilterMode.Point };

        // 병합할 레이어가 없는 경우 빈 텍스처 생성
        if (finalOrderedPixels.Count == 0)
        {
            var emptyPixels = new Color32[width * height];
            for (int i = 0; i < emptyPixels.Length; i++)
            {
                emptyPixels[i] = new Color32(0, 0, 0, 0); // 투명
            }
            mergedTexture.SetPixels32(emptyPixels);
        }
        else
        {
            mergedTexture = TextureProcessor.MergeLayers(mergedTexture, finalOrderedPixels.ToArray());
        }
        
        mergedTexture.Apply();

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
                    sprites.Add(key, Sprite.Create(mergedTexture, new Rect(j * 64, i * 64, 64, 64), new Vector2(0.5f, 0.125f), 16, 0, SpriteMeshType.FullRect));
                }
            }
        }

        var spriteLibraryAsset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();

        foreach (var sprite in sprites)
        {
            var split = sprite.Key.Split("_");
            spriteLibraryAsset.AddCategoryLabel(sprite.Value, split[0], split[1]);
        }

        if (!string.IsNullOrEmpty(parts[(int)CharacterPartsType.Cape]) && processedLayersPixels.ContainsKey("Cape"))
        {
            CapeOverlay(processedLayersPixels["Cape"]);
        }

        return spriteLibraryAsset;
    }

    private async UniTask LoadAndStoreTexture(string layerName, string address)
    {
        // Preload 모드에서는 미리 로드된 텍스처를 우선 사용
        if (CurrentMode == Mode.Preload && isPreloaded)
        {
            string preloadKey = GetPreloadKey(layerName, address);
            if (preloadedTextures.ContainsKey(preloadKey) && preloadedAddress.ContainsKey(preloadKey) && preloadedAddress[preloadKey].Equals(address))
            {
                loadedTextures[layerName] = preloadedTextures[preloadKey];
                loadedAddress[layerName] = address;
                return;
            }
        }
        
        if (loadedAddress.TryGetValue(layerName, out var existingAdress))
        {
            if (existingAdress.Equals(address))
                return;

            // OnDemand 에서는 기존 텍스처 해제
            if (CurrentMode == Mode.OnDemand)
            {
                AddressableManager.Instance.ReleaseFromTracker
                    (loadedTextures[layerName], gameObject);
            }
        }

        Texture2D texture = await AddressableManager.Instance.
            LoadAssetAsyncWithTracker<Texture2D>(address, gameObject);

        if (texture != null)
        {
            loadedTextures[layerName] = texture;
            loadedAddress[layerName] = address;
        }
        else
        {
            Logger.Error($"Character Parts Load Failed : {address}");
        }
    }

    private string GetAddressFromAddressableData(string layerName, string partName)
    {
        if (partsContainer == null)
            partsContainer = DataManager.Instance.GetDataContainer<DataCharacterParts>();
            
        if (partsContainer == null) 
            return null;

        var targetPart = partsContainer.Find(p => 
            p.PartsType.ToString() == layerName && 
            GetPartNameFromPath(p.PartsPath) == partName);
            
        if (!targetPart.IsNull)
            return targetPart.PartsPath;
            
        return null;
    }
    
    private string GetPartNameFromPath(string partsPath)
    {
        if (string.IsNullOrEmpty(partsPath))
            return string.Empty;
            
        var lastSlashIndex = partsPath.LastIndexOf('/');
        if (lastSlashIndex >= 0 && lastSlashIndex < partsPath.Length - 1)
            return partsPath.Substring(lastSlashIndex + 1);
            
        return partsPath;
    }

    private Color32[] ProcessLayerPixels(string layerName, Texture2D sourceTexture, string data, Color32[] mask)
    {
        if (sourceTexture == null) return null;

        var match = Regex.Match(data,
            @"(?<Name>[\w\- \[\]]+)(?<Paint>#\w+)?(?:/(?<H>[-\d]+):(?<S>[-\d]+):(?<V>[-\d]+))?");

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

        Color32[] pixels = sourceTexture.GetPixels32();

        if (mask != null)
        {
            for (var i = 0; i < pixels.Length; i++)
            {
                if (mask[i].a <= 0)
                {
                    pixels[i] = new Color32();
                }
                else if (mask[i] == Color.black)
                {
                    pixels[i] = mask[i];
                }
            }
        }

        if (paint != Color.white)
        {
            if (layerName == "Head" || layerName == "Body" || layerName == "Arms" || layerName == "Hair")
            {
                pixels = TextureProcessor.Repaint3C(pixels, paint, TextureProcessor.Palette);
            }
            else
            {
                for (var i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].a > 0) pixels[i] = (Color32)((Color)pixels[i] * paint);
                }
            }
        }

        if (!Mathf.Approximately(h, 0) || !Mathf.Approximately(s, 0) || !Mathf.Approximately(v, 0))
        {
            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0 && pixels[i] != Color.black)
                {
                    pixels[i] = TextureProcessor.AdjustColor(pixels[i], h, s, v);
                }
            }
        }

        pixels = TextureProcessor.ApplyPalette(pixels, TextureProcessor.Palette);

        return pixels;
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
        
        var preloadTasks = new List<UniTask>();
        var preloadParts = partsContainer.FindAll(x => !x.IsNull && 
            (x.Category == CharacterPartsCategory.Race || x.Category == CharacterPartsCategory.Hair));
        
        foreach (var part in preloadParts)
        {
            if (string.IsNullOrEmpty(part.PartsPath))
                continue;

            preloadTasks.Add(PreloadTexture(part.PartsType.ToString(), part.PartsPath, part.PartsPath));
        }
        
        await UniTask.WhenAll(preloadTasks);
        isPreloaded = true;
    }
    
    private async UniTask PreloadTexture(string layerName, string partName, string address)
    {
        string key = GetPreloadKey(layerName, address);
        
        if (preloadedTextures.ContainsKey(key))
            return;
            
        try
        {
            Texture2D texture = await AddressableManager.Instance.LoadAssetAsyncWithTracker<Texture2D>(address, gameObject);
            
            if (texture != null)
            {
                preloadedTextures[key] = texture;
                preloadedAddress[key] = address;
            }
            else
            {
                Logger.Error($"Failed to preload texture: {address}");
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Exception during preload: {e.Message}");
        }
    }
    
    private string GetPreloadKey(string layerName, string address)
    {
        return $"{layerName}_{address}";
    }
    
    public void ResetPreload()
    {
        if (CurrentMode != Mode.Preload)
        {
            Logger.Warning("ResetPreload called but current mode is not Preload");
            return;
        }
        
        var currentlyUsedTextures = new HashSet<Texture2D>(loadedTextures.Values);
        var texturesToRelease = new List<string>();
        
        foreach (var kvp in preloadedTextures)
        {
            if (!currentlyUsedTextures.Contains(kvp.Value))
            {
                texturesToRelease.Add(kvp.Key);
                AddressableManager.Instance.ReleaseFromTracker(kvp.Value, gameObject);
            }
        }
        
        foreach (var key in texturesToRelease)
        {
            preloadedTextures.Remove(key);
            preloadedAddress.Remove(key);
        }
    }
    
    private void OnDestroy()
    {
        foreach (var texture in preloadedTextures.Values)
            AddressableManager.Instance.ReleaseFromTracker(texture, gameObject);

        AddressableManager.Instance.ReleaseGameObject(gameObject);
    }
}