using System;
using UnityEngine;

public class BattleEventTriggerUnitModel : IBaseUnitModel
{
    public Action<Collider2D> OnEventHit { get; private set; }

    public Transform FollowTarget { get; private set; }

    public bool IsFlip { get; private set; }

    public TeamTag DetectTeamTag { get; private set; }

    public void SetDetectTeamTag(TeamTag detectTeamTag)
    {
        DetectTeamTag = detectTeamTag;
    }

    public void SetOnEventHit(Action<Collider2D> onEventHit)
    {
        OnEventHit = onEventHit;
    }

    public void SetFollowTarget(Transform target)
    {
        FollowTarget = target;
    }

    public void SetFlip(bool flip)
    {
        IsFlip = flip;
    }
}
