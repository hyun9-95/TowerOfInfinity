
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// isWalkable 타일을 빨간색으로 표시함
/// </summary>
public class AStarGridViewer : MonoBehaviour
{
    private Dictionary<Vector3Int, AStarNode> nodeMap;
    private Grid layoutGrid;

    public void SetNodeMap(Dictionary<Vector3Int, AStarNode> nodeMap, Grid layoutGrid)
    {
        this.nodeMap = nodeMap;
        this.layoutGrid = layoutGrid;
    }

    private void OnDrawGizmos()
    {
        if (nodeMap == null || layoutGrid == null)
            return;

        foreach (var kvp in nodeMap)
        {
            var node = kvp.Value;
            if (!node.isWalkable)
            {
                Gizmos.color = Color.red;
                Vector3 drawPos = new Vector3(node.xPos, node.yPos, 0f);
                Vector3 drawSize = layoutGrid.cellSize;
                drawPos -= layoutGrid.cellGap / 2;
                Gizmos.DrawWireCube(drawPos, drawSize);
            }
        }
    }
}
