using System;
using UnityEngine;

public class BattleEventTriggerUnitModel : IBaseUnitModel
{
    public Action<Collider2D> OnEventHit { get; private set; }

    public Transform FollowTarget { get; private set; }

    public Func<bool> IsOverTargetCount { get; private set; }

    public bool IsComplete { get; private set; }

    public bool IsFlip { get; private set; }

    public void SetFollowTarget(Transform target)
    {
        FollowTarget = target;
    }

    public void SetOnEventHit(Action<Collider2D> onEventHit)
    {
        OnEventHit = onEventHit;
    }

    public void SetGetOverTargetCount(Func<bool> isOverTargetCount)
    {
        IsOverTargetCount = isOverTargetCount;
    }

    public void SetComplete(bool complete)
    {
        IsComplete = complete;
    }

    public void SetFlip(bool flip)
    {
        IsFlip = flip;
    }
}
