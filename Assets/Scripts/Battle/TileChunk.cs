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
    
    public int ChunkSize => chunkSize;
    public Tilemap WalkableTilemap => walkableTilemap;
    public Tilemap ObstacleTilemap => obstacleTilemap;
    
    public void SetPosition(Vector3 worldPos)
    {
        transform.position = worldPos;
    }
    
    public void SetPosition(Vector2Int gridPos, int chunkSize)
    {
        Vector3 worldPos = new Vector3(gridPos.x * chunkSize, gridPos.y * chunkSize, 0);
        transform.position = worldPos;
    }
}