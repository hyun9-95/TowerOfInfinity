using UnityEngine;

public interface IPathFinder
{
    public virtual void OnStopPathFind() { }
    public virtual void RecalculatePath() { }
}
