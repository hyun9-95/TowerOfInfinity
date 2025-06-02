using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using Unity.VisualScripting;
using UnityEngine;

public class UnloadResourcesTest : MonoBehaviour
{
    [SerializeField]
    private CharacterBuilder characterBuilder;

    private void Update()
    {
        if (characterBuilder != null)
        {
            long beforeMemory = System.GC.GetTotalMemory(false);

            UnloadCollection(characterBuilder.SpriteCollection);
            GameObject.DestroyImmediate(characterBuilder);

            long afterMemory = System.GC.GetTotalMemory(true);
            Debug.Log($"Memory difference: {(beforeMemory - afterMemory) / 1024 / 1024}MB");
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Resources.UnloadUnusedAssets();
        }
    }

    public void UnloadCollection(SpriteCollection spriteCollection)
    {
        // 1. 각 레이어의 텍스처들 먼저 언로드
        if (spriteCollection != null && spriteCollection.Layers != null)
        {
            foreach (var layer in spriteCollection.Layers)
            {
                if (layer.Textures != null)
                {
                    foreach (var texture in layer.Textures)
                    {
                        if (texture != null)
                        {
                            Resources.UnloadAsset(texture);
                        }
                    }
                }
                // 레이어의 텍스처 리스트 참조 제거
                layer.Textures.Clear();
            }
        }

        // 2. 팔레트 텍스처 언로드
        if (spriteCollection.PaletteTexture != null)
        {
            Resources.UnloadAsset(spriteCollection.PaletteTexture);
        }

        // 3. 레이어 리스트 참조 제거
        spriteCollection.Layers.Clear();

        // 4. ScriptableObject 자체 언로드
        Resources.UnloadAsset(spriteCollection);

        // 5. 변수 참조 제거
        spriteCollection = null;

        // 6. 가비지 컬렉션 강제 실행
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
}
