
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapContextMenu
{
    // 육안으로 보이지 않지만 RuleTile로 설정되어 불필요한 참조를 가지고 있는 타일맵을 청소해줌
    [MenuItem("CONTEXT/Tilemap/Clear RuleTiles")]
    private static void ClearRuleTile(MenuCommand command)
    {
        Tilemap tilemap = command.context as Tilemap;
        if (tilemap == null)
        {
            Debug.LogError("Failed to get Tilemap component.");
            return;
        }

        int clearedCount = 0;
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile is RuleTile)
            {
                tilemap.SetTile(pos, null);
                clearedCount++;
            }
        }

        tilemap.CompressBounds();

        Debug.Log($"Cleared {clearedCount} RuleTiles from '{tilemap.name}' and compressed its bounds.", tilemap.gameObject);
    }
}
