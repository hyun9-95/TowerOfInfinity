using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChunkObstacleInfo
{
    public TileBase[] CachedTiles { get; private set; }
    
    public BoundsInt ObstacleBounds { get; private set; }
    
    public Tilemap ObstacleTilemap { get; private set; }
    
    public bool IsInitialized { get; private set; }

    public TileChunkObstacleInfo(Tilemap obstacleTilemap)
    {
        ObstacleTilemap = obstacleTilemap;

        ObstacleTilemap.CompressBounds();
        ObstacleBounds = ObstacleTilemap.cellBounds;

        if (ObstacleBounds.size.x <= 0 || ObstacleBounds.size.y <= 0)
        {
            CachedTiles = new TileBase[0];
            IsInitialized = true;
            return;
        }

        int tileCount = ObstacleBounds.size.x * ObstacleBounds.size.y;

        // 실제 장애물 타일이 배치된것만 할당한다.
        // 배치안된 인덱스는 null임.
        CachedTiles = new TileBase[tileCount];
        ObstacleTilemap.GetTilesBlockNonAlloc(ObstacleBounds, CachedTiles);

        IsInitialized = true;
    }

    public bool IsValid()
    {
        return IsInitialized && ObstacleTilemap != null && CachedTiles != null;
    }
}