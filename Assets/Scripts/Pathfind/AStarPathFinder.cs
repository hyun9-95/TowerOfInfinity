using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : IPathFinder
{
    private Func<float> onGetMoveSpeed;
    private Func<Vector3, Vector3, List<AStarNode>> onGetPaths;

    private Rigidbody2D rigidBody2D;
    private float repathCoolTime;
    private float nextNodeThreshold;
    private float repathTimer;
    private int currentPathIndex;
    private List<AStarNode> currentPath = new List<AStarNode>();

    public AStarPathFinder(Rigidbody2D rigidBody2D, float repathCoolTime, float nextNodeThreshold, Func<float> onGetMoveSpeed, Func<Vector3, Vector3, List<AStarNode>> onGetPaths)
    {
        this.rigidBody2D = rigidBody2D;
        this.repathCoolTime = repathCoolTime;
        this.nextNodeThreshold = nextNodeThreshold;
        this.onGetMoveSpeed = onGetMoveSpeed;
        this.onGetPaths = onGetPaths;
    }

    public Vector2 OnPathFind(Vector3 pos)
    {
        repathTimer += Time.fixedDeltaTime;

        bool isElapsedCoolTime = repathTimer >= repathCoolTime;

        bool needRebuildPath =
            isElapsedCoolTime ||                       // 목표 지점이 바뀜
            currentPath == null ||                      // 경로가 없거나
            currentPath.Count == 0 ||                   // 노드가 비어 있거나
            currentPathIndex >= currentPath.Count;      // 경로가 끝났음

        if (needRebuildPath)
        {
            currentPath = AStarManager.Instance.FindPath(rigidBody2D.position, pos);
            currentPathIndex = 0;
            repathTimer = 0;
        }

        Vector2 currentPos = rigidBody2D.position;
        Vector2 moveDir = Vector2.zero;

        if (currentPath == null || currentPath.Count == 0 || currentPathIndex >= currentPath.Count)
        {
            moveDir = (new Vector2(pos.x, pos.y) - currentPos).normalized;

            // 실제 이동
            rigidBody2D.MovePosition(
                currentPos + moveDir *
                onGetMoveSpeed.Invoke() * Time.fixedDeltaTime);
        }
        else
        {
            AStarNode targetNode = currentPath[currentPathIndex];
            Vector2 targetPos = new Vector2(targetNode.xPos, targetNode.yPos);
            moveDir = (targetPos - currentPos).normalized;

            // 실제 이동
            rigidBody2D.MovePosition(
                currentPos + moveDir *
                onGetMoveSpeed.Invoke() * Time.fixedDeltaTime);

            // 다음 노드로 전환
            if (Vector2.Distance(currentPos, targetPos) < nextNodeThreshold)
                currentPathIndex++;
        }

        return moveDir;
    }

    public void OnStopPathFind()
    {
        currentPath = null;
        currentPathIndex = 0;
        repathTimer = 0;
        rigidBody2D.linearVelocity = Vector2.zero;
    }

    public void RecalculatePath()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        repathTimer = repathCoolTime;
    }
}
