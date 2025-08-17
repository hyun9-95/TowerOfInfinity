using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar
{
    // 타일의 상위 Grid
    private Grid currentGrid;

    private Dictionary<Vector3Int, AStarNode> nodeMap;

    private BoundsInt currentGridBounds;

    private Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
    };

    private Vector3Int[] diagonalDirections = new Vector3Int[]
    {
            new Vector3Int(-1,  1, 0),
            new Vector3Int( 1,  1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int( 1, -1, 0)
    };

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize(BoundsInt bounds, TileChunkObstacleInfo[] obstacleInfos, Grid layoutGrid)
    {
        currentGridBounds = bounds;
        currentGrid = layoutGrid;

        // Bounds는 고정 크기이다.
        int totalCellCount = currentGridBounds.size.x * currentGridBounds.size.y;
        nodeMap = new Dictionary<Vector3Int, AStarNode>(totalCellCount);

        CreateGrid(obstacleInfos);

        // 디버깅
        CheatShowGridView();
    }

    public void UpdateObstacle(BoundsInt changedBounds, TileChunkObstacleInfo[] newObstacleInfos)
    {
        if (nodeMap == null)
            return;

        nodeMap.Clear();
        currentGridBounds = changedBounds;

        CreateGrid(newObstacleInfos);

        // 디버깅
        CheatShowGridView();
    }

    public void CreateGrid(TileChunkObstacleInfo[] obstacleInfos)
    {
        HashSet<Vector3Int> obstaclePositions = GetObstacleCellPositions(obstacleInfos);

        for (int y = currentGridBounds.yMin; y < currentGridBounds.yMax; y++)
        {
            for (int x = currentGridBounds.xMin; x < currentGridBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                // 그리드 기준 셀좌표의 중심점의 월드 좌표
                Vector3 worldPos = currentGrid.GetCellCenterWorld(cellPos);

                AStarNode node = new AStarNode
                {
                    xPos = worldPos.x,
                    yPos = worldPos.y,
                    
                    // 해당 셀 위치에 장애물 타일이 겹쳐있다면 걸을 수 없음
                    isWalkable = !obstaclePositions.Contains(cellPos)
                };

                nodeMap[cellPos] = node;
            }
        }
    }

    private HashSet<Vector3Int> GetObstacleCellPositions(TileChunkObstacleInfo[] obstacleInfos)
    {
        HashSet<Vector3Int> obstaclePositions = new HashSet<Vector3Int>();

        foreach (var obstacleInfo in obstacleInfos)
        {
            if (obstacleInfo == null || !obstacleInfo.IsValid())
                continue;

            // TilChunkObstacleInfo => 타일 청크 생성 시 변하지 않는 값들을 미리 계산해놓음

            // 장애물 타일정보
            var cachedTiles = obstacleInfo.CachedTiles;

            // 장애물 영역 Bounds
            var obstacleBounds = obstacleInfo.ObstacleBounds;

            // 장애물 타일맵
            var sourceTilemap = obstacleInfo.ObstacleTilemap;

            if (obstacleBounds.size.x <= 0 || obstacleBounds.size.y <= 0)
                continue;

           
            int index = 0;
            for (int y = obstacleBounds.yMin; y < obstacleBounds.yMax; y++)
            {
                for (int x = obstacleBounds.xMin; x < obstacleBounds.xMax; x++)
                {
                    // 실제 타일이 있는 위치만 목록에 추가
                    if (cachedTiles[index] != null)
                    {
                        Vector3Int localCellPos = new Vector3Int(x, y, 0);
                        
                        // 타일맵 셀 좌표 => 월드 좌표
                        Vector3 worldPos = sourceTilemap.CellToWorld(localCellPos);

                        // 월드 좌표를 그대로 사용하면 부동 소수점 오차 발생하므로
                        // 셀좌표로 변환해서 사용
                        Vector3Int globalCellPos = currentGrid.WorldToCell(worldPos);
                        
                        obstaclePositions.Add(globalCellPos);
                    }
                    index++;
                }
            }
        }

        return obstaclePositions;
    }

    public AStarNode GetNodeFromWorld(Vector3 worldPosition)
    {
        Vector3Int cellPos = currentGrid.WorldToCell(worldPosition);
        nodeMap.TryGetValue(cellPos, out var node);
        return node;
    }

    public List<AStarNode> GetNeighborNodes(AStarNode node, bool diagonal = false)
    {
        List<AStarNode> neighbors = new List<AStarNode>();

        // 셀 기준으로 좌표 재계산
        Vector3Int cellPos = currentGrid.WorldToCell(new Vector3(node.xPos, node.yPos));

        // 상하좌우 탐색
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = cellPos + dir;

            if (nodeMap.TryGetValue(neighborPos, out var neighbor))
                neighbors.Add(neighbor);
        }

        // 대각선 탐색
        if (!diagonal)
            return neighbors;

        foreach (var diagDireciton in diagonalDirections)
        {
            Vector3Int neighborPos = cellPos + diagDireciton;

            if (nodeMap.TryGetValue(neighborPos, out var diagonalNode))
            {
                // 대각선 방향 노드의 이웃을 검사
                // 벽을 통과하지 않도록, 상하좌우 노드가 walkable일 경우에만 대각선 이동이 가능하다.
                var diagonalNeighborNodes = GetNeighborNodes(diagonalNode, false);

                bool isValidDiagonal = true;

                foreach (var diagNeighborNode in diagonalNeighborNodes)
                {
                    if (diagNeighborNode.isWalkable == false)
                    {
                        isValidDiagonal = false;
                        break;
                    }    
                }

                if (isValidDiagonal)
                    neighbors.Add(diagonalNode);
            }
        }

        return neighbors;
    }

    public void ResetNode()
    {
        foreach (var node in nodeMap.Values)
            node.ResetCost();
    }

    #region Pathfind
    private int Heuristic(AStarNode a, AStarNode b, bool diagonal = false)
    {
        int dx = Mathf.Abs(Mathf.RoundToInt(a.xPos - b.xPos));
        int dy = Mathf.Abs(Mathf.RoundToInt(a.yPos - b.yPos));

        return diagonal
            ? 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy)
            : 10 * (dx + dy);
    }

    private List<AStarNode> CreatePath(AStarNode start, AStarNode end, bool diagonal = false)
    {
        if (start == null || end == null || !start.isWalkable || !end.isWalkable)
            return null;

        // 경로 재계산 시 마다, Node를 리셋해줘야함.
        ResetNode();

        PriorityQueue<AStarNode> openSet = new PriorityQueue<AStarNode>();
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

        start.gCost = 0;
        start.hCost = Heuristic(start, end, diagonal);
        openSet.Enqueue(start);

        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet.Dequeue();

            if (currentNode == end)
            {
                // 경로 역추적
                List<AStarNode> path = new List<AStarNode>();
                AStarNode tempNode = end;
                while (tempNode != null)
                {
                    path.Add(tempNode);
                    tempNode = tempNode.parent;
                }

                path.Reverse();
                return path;
            }

            closedSet.Add(currentNode);

            foreach (AStarNode neighbor in GetNeighborNodes(currentNode, diagonal))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int expectedGCost = currentNode.gCost + Heuristic(currentNode, neighbor, diagonal);

                if (expectedGCost < neighbor.gCost)
                {
                    neighbor.gCost = expectedGCost;
                    neighbor.hCost = Heuristic(neighbor, end, diagonal);
                    neighbor.parent = currentNode;

                    // 이미 들어있더라도 자동 우선순위 조정이 되므로 중복 체크 없이 Enqueue
                    openSet.Enqueue(neighbor);
                }
            }
        }

        return null; // 경로 없음
    }

    private AStarNode FindNearestWalkableNode(Vector3 worldPosition, int searchRadius = 3)
    {
        AStarNode startNode = GetNodeFromWorld(worldPosition);

        if (startNode != null && startNode.isWalkable)
            return startNode;

        Vector3Int originCell = currentGrid.WorldToCell(worldPosition);
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(originCell);

        HashSet<Vector3Int> searchedCells = new HashSet<Vector3Int>();
        searchedCells.Add(originCell);

        while(queue.Count > 0)
        {
            Vector3Int currentCell = queue.Dequeue();

            // 가장 먼저 발견된 걸을 수 있는 노드 반환
            if (nodeMap.TryGetValue(currentCell, out AStarNode node) && node.isWalkable)
                return node;

            // 현재 셀로부터의 거리가 탐색 반경을 초과하면 더 이상 탐색하지 않음
            var cellDistance = (currentCell - originCell).sqrMagnitude;
            var searchRadiusSqr = searchRadius * searchRadius;

            if (cellDistance > searchRadiusSqr)
                continue;

            // 이웃 셀 탐색
            foreach (var dir in directions)
            {
                Vector3Int neighborCell = currentCell + dir;
                if (!searchedCells.Contains(neighborCell))
                {
                    searchedCells.Add(neighborCell);
                    queue.Enqueue(neighborCell);
                }
            }
        }

        return null; // 주변에서 걸을 수 있는 노드를 찾지 못함
    }

    public List<AStarNode> CreatePath(Vector3 start, Vector3 end)
    {
        // StartNode는 유효하지 않다면 타겟 방향으로 그냥 이동하기 때문에 인접노드를 검색할 필요는 없음.
        AStarNode startNode = GetNodeFromWorld(start);
        AStarNode endNode = FindNearestWalkableNode(end);

        return CreatePath(startNode, endNode, true);
    }

    private void CheatShowGridView()
    {
#if CHEAT && UNITY_EDITOR
        if (CheatManager.CheatConfig.IsDebugAStar)
        {
            var viewer = GameManager.Instance.gameObject.GetComponent<AStarGridViewer>();
            if (viewer == null)
                viewer = GameManager.Instance.gameObject.AddComponent<AStarGridViewer>();

            viewer.SetNodeMap(nodeMap, currentGrid);
        }
#endif
    }
    #endregion
}