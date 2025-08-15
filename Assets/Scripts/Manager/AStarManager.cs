using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarManager : BaseManager<AStarManager>
{
    private AStar aStar = new();

    public void Initialize(Tilemap[] walkableMaps, Tilemap[] obstacleMaps, Grid layoutGrid)
    {
        aStar.Initialize(walkableMaps, obstacleMaps, layoutGrid);
    }

    public void RecreateGrid(Tilemap[] walkableMaps, Tilemap[] obstacleMaps, Grid layoutGrid)
    {
        aStar.ReCreateGrid(walkableMaps, obstacleMaps, layoutGrid);
    }

    public List<AStarNode> FindPath(Vector3 start, Vector3 end)
    {
        return aStar.CreatePath(start, end);
    }

    public bool IsWalkablePos(Vector3 pos)
    {
        var node = aStar.GetNodeFromWorld(pos);

        return node != null && node.isWalkable;
    }
}