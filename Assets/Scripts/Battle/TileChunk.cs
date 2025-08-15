using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChunk : AddressableMono
{
    [SerializeField]
    private int chunkSize;
    
    [SerializeField]
    private Tilemap walkableTilemap;
    
    [SerializeField]
    private Tilemap obstacleTilemap;
    
    // 장애물 정보 캐시
    private TileChunkObstacleInfo cachedObstacleInfo;
    
    public int ChunkSize => chunkSize;
    public Tilemap WalkableTilemap => walkableTilemap;
    public Tilemap ObstacleTilemap => obstacleTilemap;
    
    // 생성 시 캐싱하는 장애물 타일, 영역 정보
    public TileChunkObstacleInfo GetObstacleInfo()
    {
        if (cachedObstacleInfo == null)
            cachedObstacleInfo = new TileChunkObstacleInfo(obstacleTilemap);

        return cachedObstacleInfo;
    }
    
    public void SetGridPosition(Vector2Int gridPos, int chunkSize)
    {
        Vector3 worldPos = new Vector3(gridPos.x * chunkSize, gridPos.y * chunkSize, 0);
        transform.position = worldPos;
    }
}