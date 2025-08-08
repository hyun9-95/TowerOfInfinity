using System;
using UnityEngine;

public class BattleEventTriggerUnitModel : IBaseUnitModel
{
    /// <summary>
    /// 유닛에서 타겟을 검출할 때.
    /// </summary>
    public Action<Collider2D> OnEventHit { get; private set; }

    /// <summary>
    /// 타겟을 트리거에서 이미 알고 있을 때
    /// </summary>
    public Action OnEventSend { get; private set; }

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

    public void SetOnEventSend(Action onEventSend)
    {
        OnEventSend = onEventSend;
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
