using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : IPathFinder
{
    private Func<float> onGetMoveSpeed;

    private Rigidbody2D rigidBody2D;
    private float nextNodeThreshold;
    private int currentPathIndex;
    private List<AStarNode> currentPath = new List<AStarNode>();
    private Vector3 currentTargetPosition;

    public AStarPathFinder(Rigidbody2D rigidBody2D, float nextNodeThreshold, Func<float> onGetMoveSpeed)
    {
        this.rigidBody2D = rigidBody2D;
        this.nextNodeThreshold = nextNodeThreshold;
        this.onGetMoveSpeed = onGetMoveSpeed;
    }

    public void OnPathFind(Vector3 targetPosition)
    {
        currentTargetPosition = targetPosition;
        currentPath = AStarManager.Instance.FindPath(rigidBody2D.position, targetPosition);

        if (currentPath != null && currentPath.Count > 0)
        {
            // 전진하지 못하고 와리가리하는 상황 방지를 위해 필요함!
            int closestNodeIndex = 0;
            float minSqrDistance = float.MaxValue;
            for (int i = 0; i < currentPath.Count; i++)
            {
                float sqrDistance = (rigidBody2D.position - (Vector2)currentPath[i].WorldPos).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    closestNodeIndex = i;
                }
            }

            // 가장 가까운 노드가 경로의 마지막 노드가 아니라면, 전진 방향을 확인한다.
            if (closestNodeIndex < currentPath.Count - 1)
            {
                Vector2 closestNodePos = currentPath[closestNodeIndex].WorldPos;
                Vector2 nextNodePos = currentPath[closestNodeIndex + 1].WorldPos;

                // 가장 가까운 노드에서, 다음에 이동할 노드의 방향
                Vector2 pathDir = (nextNodePos - closestNodePos).normalized;

                // 현재 위치에서, 가장 가까운 노드의 방향
                Vector2 closestNodeDir = (closestNodePos - rigidBody2D.position).normalized;

                // 경로 진행 방향과, 현재 위치에서 가장 가까운 노드로의 방향을 내적.
                // 값이 음수이면, 가장 가까운 노드가 캐릭터의 뒤에 있다는 의미. 그 다음 노드를 목표로 삼는다.
                if (Vector2.Dot(pathDir, closestNodeDir) < 0)
                {
                    currentPathIndex = closestNodeIndex + 1;
                }
                else
                {
                    currentPathIndex = closestNodeIndex;
                }
            }
            else
            {
                // 가장 가까운 노드가 마지막 노드이면, 그냥 그 노드를 목표로 삼는다.
                currentPathIndex = closestNodeIndex;
            }
        }
        else
        {
            currentPathIndex = 0;
        }
    }

    /// <summary>
    /// 계산된 경로 따라 이동.. 이동 방향 반환
    /// </summary>
    /// <returns></returns>
    public Vector2 OnMoveAlongPath()
    {
#if UNITY_EDITOR
        if (GameManager.Config.IsDebugAStar && currentPath != null)
        {
            for (int i = 0; i < currentPath.Count - 1; i++)
                Debug.DrawLine(currentPath[i].WorldPos, currentPath[i + 1].WorldPos, Color.yellow, FloatDefine.ASTAR_REPATH_COOLTIME);
        }
#endif

        Vector2 currentPos = rigidBody2D.position;
        Vector2 moveDir = Vector2.zero;

        if (currentPath == null || currentPath.Count == 0 || currentPathIndex >= currentPath.Count)
        {
            // 경로를 못찾았다면 타겟방향으로 이동
            // RepositionTile에 의해 타일이 재배치되는 경우, 현재 타일을 밟고 있지 않아서 경로를 못찾을 수 있다.
            Vector2 targetPos = currentTargetPosition;
            moveDir = (targetPos - currentPos).normalized;
            rigidBody2D.MovePosition(
                currentPos + onGetMoveSpeed.Invoke() * Time.fixedDeltaTime * moveDir);
        }
        else
        {
            AStarNode targetNode = currentPath[currentPathIndex];
            Vector2 targetPos = new Vector2(targetNode.xPos, targetNode.yPos);
            moveDir = (targetPos - currentPos).normalized;

            Vector2 beforeMoveDelta = targetPos - currentPos;

            rigidBody2D.MovePosition(
                currentPos + onGetMoveSpeed.Invoke() * Time.fixedDeltaTime * moveDir);

            Vector2 afterMoveDelta = targetPos - rigidBody2D.position;

            // 1. 일정거리만큼 가까워졌거나
            // 2. 이미 다음 목표 노드를 지나쳤다면
            // => 다음 경로를 찾는다.
            if (afterMoveDelta.magnitude < nextNodeThreshold || Vector2.Dot(beforeMoveDelta, afterMoveDelta) < 0)
                currentPathIndex++;
        }

        return moveDir;
    }

    public void OnStopPathFind()
    {
        currentPath = null;
        currentPathIndex = 0;
    }
}