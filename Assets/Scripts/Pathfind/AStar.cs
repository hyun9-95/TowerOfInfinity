using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar
{
    // 걸을 수 있는 타일
    private Tilemap[] walkableMaps;

    // 장애물 타일
    private Tilemap[] obstacleMaps;

    // 타일의 상위 Grid
    private Grid layoutGrid;

    private Dictionary<Vector3Int, AStarNode> nodeMap;

    private BoundsInt gridBounds;

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

    public void Initialize(Tilemap[] walkableMaps, Tilemap[] obstacleMaps, Grid layoutGrid)
    {
        this.walkableMaps = walkableMaps;
        this.obstacleMaps = obstacleMaps;
        this.layoutGrid = layoutGrid;
    }

    public void CreateGrid()
    {
        gridBounds = CalculateMergedWorldBounds();
        nodeMap = new Dictionary<Vector3Int, AStarNode>();

        for (int y = gridBounds.yMin; y < gridBounds.yMax; y++)
        {
            for (int x = gridBounds.xMin; x < gridBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                Vector3 worldPos = layoutGrid.GetCellCenterWorld(cellPos);

                AStarNode node = new AStarNode
                {
                    xPos = worldPos.x,
                    yPos = worldPos.y,
                    isWalkable = true
                };

                foreach (var obstacle in obstacleMaps)
                {
                    if (obstacle == null)
                        continue;

                    Vector3Int localCell = obstacle.WorldToCell(worldPos);
                    if (obstacle.HasTile(localCell))
                    {
                        // 장애물 타일이 있을 경우 NOT WALKABLE
                        node.isWalkable = false;
                        break;
                    }
                }

                nodeMap[cellPos] = node;
            }
        }

#if CHEAT
        if (CheatManager.CheatConfig.IsDebugAStar)
        {
            var viewer = GameManager.Instance.gameObject.GetComponent<AStarGridViewer>();
            if (viewer == null)
                viewer = GameManager.Instance.gameObject.AddComponent<AStarGridViewer>();

            viewer.SetNodeMap(nodeMap, layoutGrid);
        }
#endif
    }

    private BoundsInt CalculateMergedWorldBounds()
    {
        if (walkableMaps == null || walkableMaps.Length == 0)
            return new BoundsInt();

        Bounds mergedWorldBounds = new Bounds();

        for (int i = 0; i < walkableMaps.Length; i++)
        {
            var map = walkableMaps[i];
            map.CompressBounds();
            var cellBounds = map.cellBounds;

            Vector3 worldMin = map.CellToWorld(cellBounds.min);
            Vector3 worldMax = map.CellToWorld(cellBounds.max);

            Bounds worldBounds = new Bounds();
            worldBounds.SetMinMax(worldMin, worldMax);

            if (i == 0)
                mergedWorldBounds = worldBounds;
            else
                mergedWorldBounds.Encapsulate(worldBounds);
        }

        Vector3Int cellMin = layoutGrid.WorldToCell(mergedWorldBounds.min);
        Vector3Int cellMax = layoutGrid.WorldToCell(mergedWorldBounds.max);

        return new BoundsInt(cellMin, cellMax - cellMin);
    }

    public AStarNode GetNodeFromWorld(Vector3 worldPosition)
    {
        Vector3Int cellPos = layoutGrid.WorldToCell(worldPosition);
        nodeMap.TryGetValue(cellPos, out var node);
        return node;
    }

    public List<AStarNode> GetNeighborNodes(AStarNode node, bool diagonal = false)
    {
        List<AStarNode> neighbors = new List<AStarNode>();

        // 셀 기준으로 좌표 재계산
        Vector3Int cellPos = layoutGrid.WorldToCell(new Vector3(node.xPos, node.yPos));

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
            node.Reset();
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

        Vector3Int originCell = layoutGrid.WorldToCell(worldPosition);
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
            if (Vector3Int.Distance(originCell, currentCell) > searchRadius)
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
    #endregion
}