using System;
using UnityEngine;

public class AStarNode : IComparable<AStarNode>
{
    public Vector3 WorldPos => new Vector3(xPos, yPos, 0);

    public float xPos;
    public float yPos;
    public bool isWalkable;
    public AStarNode parent;

    // 시작점 => 현재 노드까지의 비용
    public int gCost = int.MaxValue;

    // 현재 노드 => 목표 노드까지 예상 비용
    public int hCost;

    // 총 비용 => 이미 온 경로 비용 + 예상 비용
    public int fCost => gCost + hCost;

    // 비용이 낮은 순으로 정렬
    public int CompareTo(AStarNode other)
    {
        int result = fCost.CompareTo(other.fCost);

        if (result == 0)
            result = hCost.CompareTo(other.hCost); // fCost가 같을 때 hCost 낮은 쪽 우선

        return result;
    }

    public void ResetCost()
    {
        parent = null;
        gCost = int.MaxValue;
        hCost = 0;
    }
}