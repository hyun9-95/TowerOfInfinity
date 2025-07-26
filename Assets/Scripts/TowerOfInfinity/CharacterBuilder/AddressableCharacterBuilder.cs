using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace TowerOfInfinity.CharacterBuilder
{
    public class AddressableCharacterBuilder : MonoBehaviour
    {
        public AddressableSpriteData AddressableData;
        public SpriteLibrary spriteLibrary;
        public string Head = "Human";
        public string Ears = "Human";
        public string Eyes = "Human";
        public string Body = "Human";
        public string Hair;
        public string Armor;
        public string Helmet;
        public string Weapon;
        public string Firearm;
        public string Shield;
        public string Cape;
        public string Back;
        public string Mask;
        public string Horns;

        private Texture2D _mergedTexture;
        private Dictionary<string, Sprite> _sprites;

        private Dictionary<string, Texture2D> _loadedTextures = new Dictionary<string, Texture2D>();

        public void Awake()
        {
            Rebuild().Forget();
        }

        public async UniTask Rebuild(string changed = null, bool forceMerge = false)
        {
            foreach (var texture in _loadedTextures.Values)
            {
                AddressableTextureLoader.ReleaseTexture(texture);
            }
            _loadedTextures.Clear();

            var width = 576;
            var height = 928;

            var layersToLoad = new Dictionary<string, string>();
            var layerPartData = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(Head)) { layersToLoad["Head"] = GetAddressFromAddressableData("Head", Head); layerPartData["Head"] = Head; }
            if (!string.IsNullOrEmpty(Ears)) { layersToLoad["Ears"] = GetAddressFromAddressableData("Ears", Ears); layerPartData["Ears"] = Ears; }
            if (!string.IsNullOrEmpty(Eyes)) { layersToLoad["Eyes"] = GetAddressFromAddressableData("Eyes", Eyes); layerPartData["Eyes"] = Eyes; }
            if (!string.IsNullOrEmpty(Body)) { layersToLoad["Body"] = GetAddressFromAddressableData("Body", Body); layerPartData["Body"] = Body; }
            if (!string.IsNullOrEmpty(Hair)) { layersToLoad["Hair"] = GetAddressFromAddressableData("Hair", Hair); layerPartData["Hair"] = Hair; }
            if (!string.IsNullOrEmpty(Armor)) { layersToLoad["Armor"] = GetAddressFromAddressableData("Armor", Armor); layerPartData["Armor"] = Armor; }
            if (!string.IsNullOrEmpty(Helmet)) { layersToLoad["Helmet"] = GetAddressFromAddressableData("Helmet", Helmet); layerPartData["Helmet"] = Helmet; }
            if (!string.IsNullOrEmpty(Weapon)) { layersToLoad["Weapon"] = GetAddressFromAddressableData("Weapon", Weapon); layerPartData["Weapon"] = Weapon; }
            if (!string.IsNullOrEmpty(Firearm)) { layersToLoad["Firearm"] = GetAddressFromAddressableData("Firearm", Firearm); layerPartData["Firearm"] = Firearm; }
            if (!string.IsNullOrEmpty(Shield)) { layersToLoad["Shield"] = GetAddressFromAddressableData("Shield", Shield); layerPartData["Shield"] = Shield; }
            if (!string.IsNullOrEmpty(Cape)) { layersToLoad["Cape"] = GetAddressFromAddressableData("Cape", Cape); layerPartData["Cape"] = Cape; }
            if (!string.IsNullOrEmpty(Back)) { layersToLoad["Back"] = GetAddressFromAddressableData("Back", Back); layerPartData["Back"] = Back; }
            if (!string.IsNullOrEmpty(Mask)) { layersToLoad["Mask"] = GetAddressFromAddressableData("Mask", Mask); layerPartData["Mask"] = Mask; }
            if (!string.IsNullOrEmpty(Horns)) { layersToLoad["Horns"] = GetAddressFromAddressableData("Horns", Horns); layerPartData["Horns"] = Horns; }

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

            Func<string, string, Color32[], string, Color32[]> getProcessedPixels = (layer, partData, mask, changedParam) =>
            {
                if (_loadedTextures.TryGetValue(layer, out Texture2D texture))
                {
                    return ProcessLayerPixels(layer, texture, partData, mask, changedParam);
                }
                return null;
            };

            if (layerPartData.ContainsKey("Back")) processedLayersPixels.Add("Back", getProcessedPixels("Back", layerPartData["Back"], null, changed));
            if (layerPartData.ContainsKey("Shield")) processedLayersPixels.Add("Shield", getProcessedPixels("Shield", layerPartData["Shield"], null, changed));

            if (layerPartData.ContainsKey("Body"))
            {
                processedLayersPixels.Add("Body", getProcessedPixels("Body", layerPartData["Body"], null, changed));

                if (Firearm == "" && layerPartData.ContainsKey("Arms"))
                {
                    processedLayersPixels.Add("Arms", getProcessedPixels("Arms", layerPartData["Body"], null, changed == "Body" ? "Arms" : changed));
                }
            }

            if (layerPartData.ContainsKey("Head")) processedLayersPixels.Add("Head", getProcessedPixels("Head", layerPartData["Head"], null, changed));
            if (layerPartData.ContainsKey("Ears") && (Helmet == "" || Helmet.Contains("[ShowEars]")))
            {
                processedLayersPixels.Add("Ears", getProcessedPixels("Ears", layerPartData["Ears"], null, changed));
            }

            if (layerPartData.ContainsKey("Armor"))
            {
                processedLayersPixels.Add("Armor", getProcessedPixels("Armor", layerPartData["Armor"], null, changed));

                if (Firearm == "" && layerPartData.ContainsKey("Bracers"))
                {
                    processedLayersPixels.Add("Bracers", getProcessedPixels("Bracers", layerPartData["Armor"], null, changed == "Armor" ? "Bracers" : changed));
                }
            }

            if (layerPartData.ContainsKey("Eyes")) processedLayersPixels.Add("Eyes", getProcessedPixels("Eyes", layerPartData["Eyes"], null, changed));
            if (layerPartData.ContainsKey("Hair"))
            {
                Color32[] hairMask = (Helmet == "" || !processedLayersPixels.ContainsKey("Head")) ? null : processedLayersPixels["Head"];
                processedLayersPixels.Add("Hair", getProcessedPixels("Hair", layerPartData["Hair"], hairMask, changed));
            }
            if (layerPartData.ContainsKey("Cape")) processedLayersPixels.Add("Cape", getProcessedPixels("Cape", layerPartData["Cape"], null, changed));
            if (layerPartData.ContainsKey("Helmet")) processedLayersPixels.Add("Helmet", getProcessedPixels("Helmet", layerPartData["Helmet"], null, changed));
            if (layerPartData.ContainsKey("Weapon")) processedLayersPixels.Add("Weapon", getProcessedPixels("Weapon", layerPartData["Weapon"], null, changed));

            if (layerPartData.ContainsKey("Mask")) processedLayersPixels.Add("Mask", getProcessedPixels("Mask", layerPartData["Mask"], null, changed));
            if (layerPartData.ContainsKey("Horns") && Helmet == "") processedLayersPixels.Add("Horns", getProcessedPixels("Horns", layerPartData["Horns"], null, changed));

            if (Firearm != "")
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

            if (_mergedTexture == null)
                _mergedTexture = new Texture2D(width, height) { filterMode = FilterMode.Point };

            _mergedTexture = TextureProcessor.MergeLayers(_mergedTexture, finalOrderedPixels.ToArray());
            _mergedTexture.Apply();

            if (_sprites == null)
            {
                var clipNames = new List<string> { "Idle", "Ready", "Run", "Crawl", "Climb", "Jump", "Push", "Jab", "Slash", "Shot", "Fire", "Block", "Death", "Roll" };
                clipNames.Reverse();
                _sprites = new Dictionary<string, Sprite>();

                for (var i = 0; i < clipNames.Count; i++)
                {
                    for (var j = 0; j < 9; j++)
                    {
                        var key = clipNames[i] + "_" + j;
                        _sprites.Add(key, Sprite.Create(_mergedTexture, new Rect(j * 64, i * 64, 64, 64), new Vector2(0.5f, 0.125f), 16, 0, SpriteMeshType.FullRect));
                    }
                }
            }

            var spriteLibraryAsset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();

            foreach (var sprite in _sprites)
            {
                var split = sprite.Key.Split("_");
                spriteLibraryAsset.AddCategoryLabel(sprite.Value, split[0], split[1]);
            }

            if (Cape != "" && processedLayersPixels.ContainsKey("Cape"))
            {
                CapeOverlay(processedLayersPixels["Cape"]);
            }

            spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
        }

        private async UniTask LoadAndStoreTexture(string layerName, string address)
        {
            Texture2D texture = await AddressableManager.Instance.LoadAssetAsyncWithName<Texture2D>(address);
            if (texture != null)
            {
                _loadedTextures[layerName] = texture;

                Debug.LogError($"load success: {layerName} : {address}");
            }
            else
            {
                Debug.LogError($"load failed : {address}");
            }
        }

        private string GetAddressFromAddressableData(string layerName, string partName)
        {
            if (AddressableData == null || AddressableData.Layers == null) return null;

            foreach (var layerData in AddressableData.Layers)
            {
                if (layerData.LayerName == layerName)
                {
                    if (layerData.Addresses != null && layerData.Addresses.Count > 0)
                    {
                        return layerData.Addresses.FirstOrDefault(addr => addr.Contains(partName));
                    }
                }
            }
            return null;
        }

        private Color32[] ProcessLayerPixels(string layerName, Texture2D sourceTexture, string data, Color32[] mask, string changed)
        {
            if (sourceTexture == null) return null;

            var match = Regex.Match(data, @"(?<Name>[\w\- \[\]]+)(?<Paint>#\w+)?(?:/(?<H>[-\d]+):(?<S>[-\d]+):(?<V>[-\d]+))?");

            Color paint = Color.white;
            if (match.Groups["Paint"].Success)
            {
                ColorUtility.TryParseHtmlString(match.Groups["Paint"].Value, out paint);
            }

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
            if (Cape == "") return;

            var pixels = _mergedTexture.GetPixels32();
            var width = _mergedTexture.width;
            var height = _mergedTexture.height;

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

            _mergedTexture.SetPixels32(pixels);
            _mergedTexture.Apply();
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
    }


}