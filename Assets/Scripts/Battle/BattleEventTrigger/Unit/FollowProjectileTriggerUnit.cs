using UnityEngine;

public class FollowProjectileTriggerUnit : ProjectileTriggerUnit
{
    private Transform followTarget;

    protected override void SetStartDirection()
    {
        direction = GetDirectionToTarget();
        followTarget = Model.FollowTarget;
    }

    protected override void UpdateMove()
    {
        if (followTarget == null)
        {
            base.UpdateMove();
            RotateSpriteToDirection();
            return;
        }

        UpdateFollow();
        RotateSpriteToDirection();
    }

    private void UpdateFollow()
    {
        Vector3 targetPosition = followTarget.position;
        direction = (targetPosition - transform.position).normalized;
        
        transform.position += Model.Speed * Time.fixedDeltaTime * (Vector3)direction;
    }

    private Vector2 GetDirectionToTarget()
    {
        if (followTarget == null)
            return Model.StartDirection;

        Vector3 targetPosition = followTarget.position;
        return (targetPosition - transform.position).normalized;
    }
}