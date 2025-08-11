using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class SpriteComposer
{
    static List<Sprite> GetSelectedSprites()
    {
        List<Sprite> sprites = new List<Sprite>();
        
        // 직접 선택된 스프라이트들 추가
        Sprite[] directSprites = Selection.GetFiltered<Sprite>(SelectionMode.Assets);
        sprites.AddRange(directSprites);
        
        // 선택된 텍스처들에서 스프라이트 추출
        Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        foreach (Texture2D texture in textures)
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(texturePath);
            
            foreach (Object asset in subAssets)
            {
                if (asset is Sprite sprite && sprite != null)
                {
                    sprites.Add(sprite);
                }
            }
        }
        
        return sprites;
    }

    [MenuItem("Assets/Compose Sprites to PNG", true)]
    static bool ValidateComposeSprites()
    {
        List<Sprite> allSprites = GetSelectedSprites();
        return allSprites.Count >= 2;
    }

    [MenuItem("Assets/Compose Sprites to PNG")]
    static void ComposeSprites()
    {
        List<Sprite> sprites = GetSelectedSprites();

        if (sprites.Count < 2)
        {
            EditorUtility.DisplayDialog("Error", "2개 이상의 스프라이트를 선택해주세요.", "OK");
            return;
        }

        try
        {
            EditorUtility.DisplayProgressBar("Composing Sprites", "Processing sprites...", 0f);
            
            string outputPath = ComposeSpritesToTexture(sprites);
            
            if (!string.IsNullOrEmpty(outputPath))
            {
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Complete", 
                    $"스프라이트가 합성되었습니다!\n{outputPath}", "OK");
                    
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath);
                Logger.Success($"Successfully composed {sprites.Count} sprites to {outputPath}");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "스프라이트 합성에 실패했습니다.", "OK");
            }
        }
        catch (System.Exception e)
        {
            Logger.Error($"Sprite composition failed: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"오류가 발생했습니다: {e.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    static string ComposeSpritesToTexture(List<Sprite> sprites)
    {
        List<TextureImporter> originalImporters = new List<TextureImporter>();
        List<bool> originalReadableStates = new List<bool>();
        List<TextureImporterCompression> originalCompressions = new List<TextureImporterCompression>();
        
        try
        {
            EditorUtility.DisplayProgressBar("Composing Sprites", "Preparing textures...", 0.1f);
            
            // 모든 텍스처를 읽기 가능하게 설정
            for (int i = 0; i < sprites.Count; i++)
            {
                string texturePath = AssetDatabase.GetAssetPath(sprites[i].texture);
                TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                
                if (importer == null)
                {
                    Logger.Error($"TextureImporter not found for {sprites[i].name}");
                    return null;
                }
                
                originalImporters.Add(importer);
                originalReadableStates.Add(importer.isReadable);
                originalCompressions.Add(importer.textureCompression);
                
                if (!importer.isReadable || importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.isReadable = true;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    AssetDatabase.ImportAsset(texturePath);
                }
            }
            
            EditorUtility.DisplayProgressBar("Composing Sprites", "Calculating canvas size...", 0.3f);
            
            // 캔버스 크기 계산
            Rect canvasBounds = CalculateCanvasBounds(sprites);
            int canvasWidth = Mathf.CeilToInt(canvasBounds.width);
            int canvasHeight = Mathf.CeilToInt(canvasBounds.height);
            
            Logger.Log($"Canvas size: {canvasWidth}x{canvasHeight}");
            
            EditorUtility.DisplayProgressBar("Composing Sprites", "Creating composite texture...", 0.5f);
            
            // 합성 텍스처 생성
            Texture2D compositeTexture = new Texture2D(canvasWidth, canvasHeight, TextureFormat.RGBA32, false);
            Color[] clearPixels = new Color[canvasWidth * canvasHeight];
            for (int i = 0; i < clearPixels.Length; i++)
                clearPixels[i] = Color.clear;
            compositeTexture.SetPixels(clearPixels);
            
            EditorUtility.DisplayProgressBar("Composing Sprites", "Compositing sprites...", 0.7f);
            
            // 스프라이트 합성 (선택 순서대로)
            for (int i = 0; i < sprites.Count; i++)
            {
                float progress = 0.7f + (0.2f * i / sprites.Count);
                EditorUtility.DisplayProgressBar("Composing Sprites", $"Compositing {sprites[i].name}...", progress);
                
                CompositeSprite(compositeTexture, sprites[i], canvasBounds);
            }
            
            compositeTexture.Apply();
            
            EditorUtility.DisplayProgressBar("Composing Sprites", "Saving file...", 0.9f);
            
            // 파일 저장
            string outputPath = SaveCompositeTexture(compositeTexture, sprites[0]);
            
            Object.DestroyImmediate(compositeTexture);
            
            return outputPath;
        }
        finally
        {
            // 원본 설정 복원
            for (int i = 0; i < originalImporters.Count; i++)
            {
                if (i < sprites.Count && originalImporters[i] != null)
                {
                    bool needsReimport = false;
                    
                    if (originalImporters[i].isReadable != originalReadableStates[i])
                    {
                        originalImporters[i].isReadable = originalReadableStates[i];
                        needsReimport = true;
                    }
                    
                    if (originalImporters[i].textureCompression != originalCompressions[i])
                    {
                        originalImporters[i].textureCompression = originalCompressions[i];
                        needsReimport = true;
                    }
                    
                    if (needsReimport)
                    {
                        string texturePath = AssetDatabase.GetAssetPath(sprites[i].texture);
                        AssetDatabase.ImportAsset(texturePath);
                    }
                }
            }
        }
    }
    
    static Rect CalculateCanvasBounds(List<Sprite> sprites)
    {
        if (sprites.Count == 0)
            return new Rect(0, 0, 256, 256);
        
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        
        foreach (Sprite sprite in sprites)
        {
            Rect rect = sprite.rect;
            Vector2 pivot = sprite.pivot;
            
            // 피벗을 기준으로 실제 위치 계산
            float leftX = -pivot.x;
            float rightX = rect.width - pivot.x;
            float bottomY = -pivot.y;
            float topY = rect.height - pivot.y;
            
            minX = Mathf.Min(minX, leftX);
            maxX = Mathf.Max(maxX, rightX);
            minY = Mathf.Min(minY, bottomY);
            maxY = Mathf.Max(maxY, topY);
        }
        
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
    
    static void CompositeSprite(Texture2D canvas, Sprite sprite, Rect canvasBounds)
    {
        Rect spriteRect = sprite.rect;
        Vector2 pivot = sprite.pivot;
        
        // 스프라이트의 픽셀 데이터 가져오기
        Color[] spritePixels = sprite.texture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            (int)spriteRect.width,
            (int)spriteRect.height
        );
        
        // 캔버스 상의 위치 계산
        int startX = Mathf.RoundToInt(-pivot.x - canvasBounds.x);
        int startY = Mathf.RoundToInt(-pivot.y - canvasBounds.y);
        
        // 픽셀별로 알파 블렌딩
        for (int y = 0; y < spriteRect.height; y++)
        {
            for (int x = 0; x < spriteRect.width; x++)
            {
                int canvasX = startX + x;
                int canvasY = startY + y;
                
                if (canvasX >= 0 && canvasX < canvas.width && canvasY >= 0 && canvasY < canvas.height)
                {
                    Color spritePixel = spritePixels[y * (int)spriteRect.width + x];
                    
                    if (spritePixel.a > 0)
                    {
                        Color canvasPixel = canvas.GetPixel(canvasX, canvasY);
                        Color blended = BlendPixels(canvasPixel, spritePixel);
                        canvas.SetPixel(canvasX, canvasY, blended);
                    }
                }
            }
        }
    }
    
    static Color BlendPixels(Color background, Color foreground)
    {
        if (foreground.a >= 1.0f)
            return foreground;
        
        if (foreground.a <= 0.0f)
            return background;
        
        float invAlpha = 1.0f - foreground.a;
        return new Color(
            foreground.r * foreground.a + background.r * background.a * invAlpha,
            foreground.g * foreground.a + background.g * background.a * invAlpha,
            foreground.b * foreground.a + background.b * background.a * invAlpha,
            foreground.a + background.a * invAlpha
        );
    }
    
    static string SaveCompositeTexture(Texture2D texture, Sprite firstSprite)
    {
        string originalPath = AssetDatabase.GetAssetPath(firstSprite.texture);
        string folderPath = Path.GetDirectoryName(originalPath);
        string timeStamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Composed_{firstSprite.name}_{timeStamp}.png";
        string fullPath = Path.Combine(folderPath, fileName).Replace("\\", "/");
        
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(fullPath, pngData);
        
        AssetDatabase.Refresh();
        
        // 생성된 PNG를 스프라이트로 설정
        SetupCompositeSprite(fullPath, firstSprite);
        
        return fullPath;
    }
    
    static void SetupCompositeSprite(string assetPath, Sprite referenceSprite)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        TextureImporter referenceImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(referenceSprite.texture)) as TextureImporter;
        
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            
            if (referenceImporter != null)
            {
                importer.spritePixelsPerUnit = referenceImporter.spritePixelsPerUnit;
                importer.filterMode = referenceImporter.filterMode;
                importer.textureCompression = referenceImporter.textureCompression;
                importer.mipmapEnabled = referenceImporter.mipmapEnabled;
                importer.wrapMode = referenceImporter.wrapMode;
                importer.maxTextureSize = referenceImporter.maxTextureSize;
            }
            else
            {
                importer.spritePixelsPerUnit = referenceSprite.pixelsPerUnit;
            }
            
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}