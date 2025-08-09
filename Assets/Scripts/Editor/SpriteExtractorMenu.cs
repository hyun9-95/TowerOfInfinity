using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExtractorMenu
{
    [MenuItem("Assets/Extract Sprites to PNG", true)]
    static bool ValidateExtractSprites()
    {
        return Selection.activeObject is Sprite;
    }

    [MenuItem("Assets/Extract Sprites to PNG")]
    static void ExtractSprites()
    {
        int extractedCount = 0;

        foreach (Object obj in Selection.objects)
        {
            if (obj is Sprite sprite)
            {
                // 원본 스프라이트와 같은 폴더에 저장
                string originalPath = AssetDatabase.GetAssetPath(sprite.texture);
                string folderPath = Path.GetDirectoryName(originalPath);
                
                if (ExtractSprite(sprite, folderPath))
                {
                    extractedCount++;
                    Debug.Log($"Extracted: {sprite.name} to {folderPath}");
                }
            }
        }

        if (extractedCount > 0)
        {
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Complete", 
                $"{extractedCount}개 스프라이트가 추출되었습니다!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "추출할 수 있는 스프라이트가 없습니다.", "OK");
        }
    }

    static bool ExtractSprite(Sprite sprite, string folderPath)
    {
        // 원본 텍스쳐 읽기 권한 확인
        string texturePath = AssetDatabase.GetAssetPath(sprite.texture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

        if (textureImporter == null)
        {
            Debug.LogError($"TextureImporter를 찾을 수 없습니다: {sprite.name}");
            return false;
        }

        bool wasReadable = textureImporter.isReadable;
        TextureImporterCompression originalCompression = textureImporter.textureCompression;

        // 읽기 권한 및 압축 해제
        if (!wasReadable || originalCompression != TextureImporterCompression.Uncompressed)
        {
            textureImporter.isReadable = true;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(texturePath);
        }

        try
        {
            // 스프라이트 영역만 추출
            Rect spriteRect = sprite.rect;
            Texture2D newTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height, TextureFormat.RGBA32, false);

            Color[] pixels = sprite.texture.GetPixels(
                (int)spriteRect.x,
                (int)spriteRect.y,
                (int)spriteRect.width,
                (int)spriteRect.height
            );

            newTexture.SetPixels(pixels);
            newTexture.Apply();

            // PNG로 저장
            byte[] pngData = newTexture.EncodeToPNG();
            string fileName = $"{sprite.name}.png";
            string fullPath = Path.Combine(folderPath, fileName);

            // 중복 파일명 처리
            int counter = 1;
            while (File.Exists(fullPath))
            {
                fileName = $"{sprite.name}_{counter}.png";
                fullPath = Path.Combine(folderPath, fileName);
                counter++;
            }

            File.WriteAllBytes(fullPath, pngData);

            // 추출된 PNG를 스프라이트로 설정 (원본 설정 따라가기)
            AssetDatabase.Refresh();
            SetupExtractedSprite(fullPath, sprite, textureImporter);

            // 메모리 해제
            Object.DestroyImmediate(newTexture);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"스프라이트 추출 실패 ({sprite.name}): {e.Message}");
            return false;
        }
        finally
        {
            // 원래 설정 복원
            if (!wasReadable || originalCompression != TextureImporterCompression.Uncompressed)
            {
                textureImporter.isReadable = wasReadable;
                textureImporter.textureCompression = originalCompression;
                AssetDatabase.ImportAsset(texturePath);
            }
        }
    }

    static void SetupExtractedSprite(string assetPath, Sprite originalSprite, TextureImporter originalImporter)
    {
        TextureImporter newImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        
        if (newImporter != null)
        {
            // 원본 설정 복사
            newImporter.textureType = TextureImporterType.Sprite;
            newImporter.spriteImportMode = SpriteImportMode.Single;
            newImporter.spritePixelsPerUnit = originalSprite.pixelsPerUnit;
            newImporter.filterMode = originalImporter.filterMode;
            newImporter.textureCompression = originalImporter.textureCompression;
            newImporter.mipmapEnabled = originalImporter.mipmapEnabled;
            newImporter.wrapMode = originalImporter.wrapMode;
            newImporter.maxTextureSize = originalImporter.maxTextureSize;
            
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}