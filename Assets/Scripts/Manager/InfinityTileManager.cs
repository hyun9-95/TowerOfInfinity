using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;

public class InfinityTileManager : BaseMonoManager<InfinityTileManager>
{
    [SerializeField]
    private string[] tileChunkAddresses;
    
    [SerializeField]
    private int gridSize = 3;
    
    [SerializeField]
    private Grid grid;
    
    private Transform playerTransform;
    private int chunkSize;
    private bool isInitialized = false;
    
    private Dictionary<Vector2Int, TileChunk> activeTiles = new Dictionary<Vector2Int, TileChunk>();
    private Vector2Int lastPlayerGridPos = Vector2Int.zero;
    
    private Queue<TileChunk> availableTiles = new();

    private bool useAStar = false;
    
    public async UniTask Prepare(Transform player, bool useAstar)
    {
        playerTransform = player;
        useAStar = useAstar;
        
        if (tileChunkAddresses == null || tileChunkAddresses.Length == 0)
        {
            Logger.Null("Tile Chunk Address");
            return;
        }
        
        if (grid == null)
        {
            Logger.Null("Grid");
            return;
        }
        
        await CreateChunkTiles();

        UpdateTileGrid();

        if (!AStarManager.Instance.IsWalkablePos(playerTransform.position))
            RepositionPlayer();

    }

    // 타일 생성 후 플레이어 위치가 걸을 수 없는 경우 재배치
    private void RepositionPlayer()
    {
        int safeCount = 100;
        int tryCount = 0;
        float baseRadius = 5f;
        float radiusIncrease = 0.05f;

        while (tryCount < safeCount)
        {
            Vector2 playerPos = playerTransform.position;
            playerPos = playerPos.RandomInCircle(baseRadius + (tryCount * radiusIncrease));
            playerTransform.position = playerPos;

            tryCount++;

            if (AStarManager.Instance.IsWalkablePos(playerTransform.position))
            {
                Logger.Log($"Reposition Player .. break at : {tryCount}");
                return;
            }
        }

        Logger.Error($"Reposition Player Failed at : {tryCount}");
    }
    
    private async UniTask CreateChunkTiles()
    {
        // ChunkSize 기준 => 첫번째 타일 청크사이즈
        // 사이즈가 다르면 안된다.
        var firstChunk = await CreateRandomTileChunk();

        if (firstChunk != null)
        {
            chunkSize = firstChunk.ChunkSize;
            availableTiles.Enqueue(firstChunk);
            
            // 그리드에 필요한 나머지 TileChunk를 미리 랜덤 생성
            int totalNeeded = gridSize * gridSize;
            for (int i = 1; i < totalNeeded; i++)
            {
                var chunk = await CreateRandomTileChunk();
                if (chunk != null)
                {
                    availableTiles.Enqueue(chunk);

                    if (chunk.ChunkSize != chunkSize)
                        Logger.Error($"TileChunk의 사이즈가 다름! {chunk.ChunkSize} != {chunkSize}");
                }
            }
            
            isInitialized = true;
        }
    }

    private async UniTask<TileChunk> CreateRandomTileChunk()
    {
        if (tileChunkAddresses.Length == 0)
            return null;

        string randomAddress = tileChunkAddresses[Random.Range(0, tileChunkAddresses.Length)];

        var tileChunk = await AddressableManager.Instance.InstantiateAddressableMonoAsync<TileChunk>(randomAddress, grid.transform);
        tileChunk.transform.position = Vector3.zero;

        return tileChunk;
    }


    private void Update()
    {
        if (!isInitialized || playerTransform == null)
            return;
           
        Vector2Int currentPlayerGridPos = WorldToGridPosition(playerTransform.position);
        
        // 월드 기준 그리드 좌표가 변경되었다면 재배치
        if (currentPlayerGridPos != lastPlayerGridPos)
        {
            UpdateTileGrid();
            UpdateAStarGrid();
            lastPlayerGridPos = currentPlayerGridPos;
        }
    }
    
    private void UpdateTileGrid()
    {
        if (!isInitialized)
            return;
            
        Vector2Int playerGridPos = WorldToGridPosition(playerTransform.position);
        HashSet<Vector2Int> requiredPositions = new HashSet<Vector2Int>();
        
        int halfGrid = gridSize / 2;
        
        // 필요한 위치 계산
        for (int x = -halfGrid; x <= halfGrid; x++)
        {
            for (int y = -halfGrid; y <= halfGrid; y++)
            {
                Vector2Int gridPos = playerGridPos + new Vector2Int(x, y);
                requiredPositions.Add(gridPos);
            }
        }
        
        // 먼저 불필요한 타일 제거 (pool에 반환)
        List<Vector2Int> tilesToRemove = new List<Vector2Int>();
        foreach (var kvp in activeTiles)
        {
            if (!requiredPositions.Contains(kvp.Key))
            {
                tilesToRemove.Add(kvp.Key);
            }
        }
        
        foreach (var gridPos in tilesToRemove)
        {
            if (activeTiles.TryGetValue(gridPos, out TileChunk chunk))
            {
                ReturnToTilePool(chunk);
                activeTiles.Remove(gridPos);
            }
        }
        
        // 그 다음에 새로운 타일 배치 (pool에서 가져옴)
        foreach (var gridPos in requiredPositions)
        {
            if (!activeTiles.ContainsKey(gridPos))
                ReplaceTileFromPool(gridPos);
        }
        
        // AStar 그리드 업데이트
        UpdateAStarGrid();
    }
    
    private void ReplaceTileFromPool(Vector2Int gridPos)
    {
        var tileChunk = GetAvailableTileChunk();
        if (tileChunk != null)
        {
            tileChunk.SetPosition(gridPos, chunkSize);
            activeTiles[gridPos] = tileChunk;
        }
    }

    private TileChunk GetAvailableTileChunk()
    {
        if (availableTiles.Count > 0)
            return availableTiles.Dequeue();

        return null;
    }
    
    private void ReturnToTilePool(TileChunk chunk)
    {
        if (chunk != null)
            availableTiles.Enqueue(chunk);
    }
    
    private Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int gridX = Mathf.FloorToInt(worldPos.x / chunkSize);
        int gridY = Mathf.FloorToInt(worldPos.y / chunkSize);
        return new Vector2Int(gridX, gridY);
    }
    
    #region AStar Integration
    private void UpdateAStarGrid()
    {
        if (!useAStar)
            return;
            
        var allWalkableMaps = CollectActiveTilemaps(true);
        var allObstacleMaps = CollectActiveTilemaps(false);
        
        if (allWalkableMaps.Length > 0 && grid != null)
            AStarManager.Instance.Initialize(allWalkableMaps, allObstacleMaps, grid);
    }
    
    private Tilemap[] CollectActiveTilemaps(bool walkable)
    {
        List<Tilemap> tilemaps = new List<Tilemap>(activeTiles.Values.Count);
        
        foreach (var chunk in activeTiles.Values)
        {
            if (chunk != null)
            {
                var tilemap = walkable ? chunk.WalkableTilemap : chunk.ObstacleTilemap;

                if (tilemap != null)
                    tilemaps.Add(tilemap);
            }
        }
        
        return tilemaps.ToArray();
    }
    #endregion
}