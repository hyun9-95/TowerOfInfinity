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

    private Vector3Int[] diagonals = new Vector3Int[]
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

#if UNITY_EDITOR
        var viewer = GameManager.Instance.gameObject.AddComponent<AStarGridViewer>();
        viewer.SetNodeMap(nodeMap, layoutGrid);
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

        foreach (var diag in diagonals)
        {
            Vector3Int neighborPos = cellPos + diag;

            if (nodeMap.TryGetValue(neighborPos, out var neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public void ResetNode()
    {
        foreach (var node in nodeMap.Values)
        {
            node.Reset();
        }
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
        {
            return null;
        }

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

    private AStarNode FindNearestWalkableNode(Vector3 worldPosition, int searchRadius = 5)
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

            if (nodeMap.TryGetValue(currentCell, out AStarNode node) && node.isWalkable)
            {
                return node; // 가장 먼저 발견된 걸을 수 있는 노드 반환
            }

            // 현재 셀로부터의 거리가 탐색 반경을 초과하면 더 이상 탐색하지 않음
            if (Vector3Int.Distance(originCell, currentCell) > searchRadius)
            {
                continue;
            }

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
        AStarNode startNode = FindNearestWalkableNode(start);
        AStarNode endNode = FindNearestWalkableNode(end);

        return CreatePath(startNode, endNode, true);
    }
    #endregion

#if UNITY_EDITOR
    [SerializeField]
    private bool isTest;

    private AStarNode testStartNode;
    private AStarNode testEndNode;

    private void Update()
    {
        if (!isTest)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            testStartNode = GetNodeFromWorld(worldPos);

            CreateGrid();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            testEndNode = GetNodeFromWorld(worldPos);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            TestPathfind();
        }
    }


    public void TestPathfind()
    {
        List<AStarNode> path = CreatePath(testStartNode, testEndNode, true);

        if (path != null && path.Count > 1)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 startWorldPos = new Vector3(path[i].xPos, path[i].yPos, 0f);
                Vector3 endWorldPos = new Vector3(path[i + 1].xPos, path[i + 1].yPos, 0f);

                Debug.DrawLine(startWorldPos, endWorldPos, Color.cyan, 5f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (nodeMap != null && isTest)
        {
            foreach (var kvp in nodeMap)
            {
                var node = kvp.Value;
                Gizmos.color = node.isWalkable ? Color.green : Color.red;
                Vector3 drawPos = new Vector3(node.xPos, node.yPos, 0f);
                Vector3 drawSize = layoutGrid.cellSize;
                drawPos -= layoutGrid.cellGap / 2;
                Gizmos.DrawWireCube(drawPos, drawSize);
            }
        }
    }
#endif
}