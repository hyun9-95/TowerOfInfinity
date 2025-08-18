using System.Collections.Generic;
using UnityEngine;

public class AStarManager : BaseManager<AStarManager>
{
    private AStar aStar = new();

    public void Initialize(BoundsInt bounds, TileChunkObstacleInfo[] obstacleInfos, Grid layoutGrid)
    {
        aStar.Initialize(bounds, obstacleInfos, layoutGrid);
    }

    /// <summary>
    /// 장애물 영역이 변경되었을 때, isWalkable을 다시 판단
    /// </summary>
    public void UpdateObstacle(BoundsInt changedBounds, TileChunkObstacleInfo[] obstacleInfos)
    {
        aStar.UpdateObstacle(changedBounds, obstacleInfos);
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