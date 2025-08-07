using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpriteAtlasCreator
{
    [MenuItem("Assets/Create Sprite Atlas from Selection", true)]
    static bool ValidateCreateAtlasFromSelection()
    {
        if (Selection.objects.Length == 0)
            return false;

        foreach (Object obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null && importer.textureType == TextureImporterType.Sprite)
                    return true;
            }
            else if (obj is Sprite)
            {
                return true;
            }
        }

        return false;
    }

    [MenuItem("Assets/Create Sprite Atlas from Selection")]
    static void CreateAtlasFromSelection()
    {
        if (Selection.objects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "텍스쳐나 스프라이트를 선택해주세요.", "OK");
            return;
        }

        // 선택된 오브젝트들에서 스프라이트 수집
        List<Sprite> sprites = CollectSpritesFromSelection();
        
        if (sprites.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "유효한 스프라이트가 없습니다.", "OK");
            return;
        }

        // 선택된 첫 번째 오브젝트의 경로 기반으로 아틀라스 경로 생성
        string firstAssetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        string directory = Path.GetDirectoryName(firstAssetPath);
        
        // 기본 아틀라스 이름 생성
        string atlasName = "SpriteAtlas_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string atlasPath = Path.Combine(directory, atlasName + ".spriteatlas");

        CreateSpriteAtlas(sprites, atlasPath);
    }

    static List<Sprite> CollectSpritesFromSelection()
    {
        List<Sprite> sprites = new List<Sprite>();
        
        foreach (Object obj in Selection.objects)
        {
            if (obj is Sprite sprite)
            {
                sprites.Add(sprite);
            }
            else if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                
                if (importer != null && importer.textureType == TextureImporterType.Sprite)
                {
                    if (importer.spriteImportMode == SpriteImportMode.Multiple)
                    {
                        // 다중 스프라이트 텍스쳐에서 모든 스프라이트 추가
                        Sprite[] textureSprites = AssetDatabase.LoadAllAssetsAtPath(path)
                            .OfType<Sprite>()
                            .ToArray();
                        sprites.AddRange(textureSprites);
                    }
                    else
                    {
                        // 단일 스프라이트 텍스쳐
                        Sprite singleSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        if (singleSprite != null)
                            sprites.Add(singleSprite);
                    }
                }
            }
        }

        return sprites;
    }

    static void CreateSpriteAtlas(List<Sprite> sprites, string atlasPath)
    {
        try
        {
            // 스프라이트 아틀라스 생성
            SpriteAtlas atlas = new SpriteAtlas();
            
            // 아틀라스 설정
            SpriteAtlasPackingSettings packingSettings = atlas.GetPackingSettings();
            packingSettings.enableRotation = false;
            packingSettings.enableTightPacking = true;
            packingSettings.padding = 2;
            atlas.SetPackingSettings(packingSettings);

            SpriteAtlasTextureSettings textureSettings = atlas.GetTextureSettings();
            textureSettings.readable = false;
            textureSettings.generateMipMaps = false;
            textureSettings.filterMode = FilterMode.Point;
            textureSettings.anisoLevel = 0;
            atlas.SetTextureSettings(textureSettings);

            // 플랫폼 설정
            TextureImporterPlatformSettings platformSettings = atlas.GetPlatformSettings("DefaultTexturePlatform");
            platformSettings.maxTextureSize = 2048;
            platformSettings.format = TextureImporterFormat.RGBA32;
            platformSettings.textureCompression = TextureImporterCompression.Uncompressed;
            atlas.SetPlatformSettings(platformSettings);

            // 스프라이트들을 아틀라스에 추가
            Object[] spriteObjects = sprites.Cast<Object>().ToArray();
            atlas.Add(spriteObjects);

            // 아틀라스 저장 (PackAtlases 호출 제거)
            AssetDatabase.CreateAsset(atlas, atlasPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // 생성된 아틀라스를 선택
            Selection.activeObject = atlas;
            EditorGUIUtility.PingObject(atlas);

            EditorUtility.DisplayDialog("완료", 
                $"스프라이트 아틀라스가 생성되었습니다!\n" +
                $"경로: {atlasPath}\n" +
                $"포함된 스프라이트: {sprites.Count}개\n\n" +
                $"아틀라스는 빌드 시 자동으로 패킹됩니다.", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"아틀라스 생성 실패: {e.Message}");
            EditorUtility.DisplayDialog("Error", 
                $"아틀라스 생성에 실패했습니다.\n{e.Message}", "OK");
        }
    }

}