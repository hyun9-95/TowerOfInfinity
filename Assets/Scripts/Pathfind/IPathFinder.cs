using UnityEngine;

public interface IPathFinder
{
    public abstract Vector2 OnPathFind(Vector3 pos);

    public abstract void OnStopPathFind();

    public abstract void RecalculatePath();
}
