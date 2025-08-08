using UnityEngine;

public class FollowProjectileTriggerUnit : ProjectileTriggerUnit
{
    private Transform followTarget;

    protected override void SetStartDirection()
    {
        direction = GetDirectionToTarget();
    }

    protected override void UpdateMove()
    {
        if (followTarget == null)
        {
            base.UpdateMove();
            return;
        }

        UpdateLinearFollow();
        RotateSpriteToDirection();
    }

    private void UpdateLinearFollow()
    {
        Vector3 targetPosition = followTarget.position;
        direction = (targetPosition - transform.position).normalized;
        
        transform.position += Model.Speed * Time.fixedDeltaTime * (Vector3)direction;
    }

    private Vector2 GetDirectionToTarget()
    {
        if (followTarget == null)
            return Vector2.right;

        Vector3 targetPosition = followTarget.position;
        return (targetPosition - transform.position).normalized;
    }
}