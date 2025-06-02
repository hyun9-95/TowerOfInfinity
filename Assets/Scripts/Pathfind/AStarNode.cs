using System;
using UnityEngine;

public class AStarNode : IComparable<AStarNode>
{
    public Vector3 WorldPos => new Vector3(xPos, yPos, 0);

    public float xPos;
    public float yPos;
    public bool isWalkable;
    public AStarNode parent;
    public int gCost = int.MaxValue;
    public int hCost;

    public int fCost => gCost + hCost;

    public int CompareTo(AStarNode other)
    {
        int result = fCost.CompareTo(other.fCost);

        if (result == 0)
            result = hCost.CompareTo(other.hCost); // fCost가 같을 때 hCost 낮은 쪽 우선

        return result;
    }

    public void Reset()
    {
        parent = null;
        gCost = int.MaxValue;
        hCost = 0;
    }
}