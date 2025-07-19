using UnityEngine;
using UnityEngine.AI;

public class NavmeshPathFinder : IPathFinder
{
    private Transform transform;
    private NavMeshAgent agent;

    public NavmeshPathFinder(Transform transform, NavMeshAgent agent)
    {
        this.transform = transform;
        this.agent = agent;
    }

    public Vector2 OnPathFindDirection(Vector3 pos)
    {
        if (agent == null)
            return Vector2.zero;

        if (!agent.isOnNavMesh)
            return Vector2.zero;

        if (!agent.hasPath || pos != agent.destination)
            agent.SetDestination(pos);

        agent.transform.rotation = Quaternion.identity;

        Vector2 direction = pos - transform.position;

        return direction;
    }

    public void OnStopPathFind()
    {
        if (agent == null || !agent.isActiveAndEnabled)
            return;

        agent.ResetPath();
        agent.velocity = Vector2.zero;
    }

    public void RecalculatePath()
    {
    }
}
