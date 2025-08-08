using System;
using UnityEngine;

public class BattleEventTriggerUnitModel : IBaseUnitModel
{
    #region Property
    public Transform FollowTarget { get; private set; }

    public bool IsFlip { get; private set; }

    public TeamTag DetectTeamTag { get; private set; }

    public int HitCount { get; private set; }
    #endregion

    #region Value
    public int currentHitCount;

    /// <summary>
    /// 유닛에서 타겟을 검출할 때.
    /// </summary>
    private Action<Collider2D> onEventHit;

    /// <summary>
    /// 타겟을 트리거에서 이미 알고 있을 때
    /// </summary>
    public Action onEventSend { get; private set; }
    #endregion

    public void SetDetectTeamTag(TeamTag detectTeamTag)
    {
        DetectTeamTag = detectTeamTag;
    }

    public void SetOnEventHit(Action<Collider2D> onEventHit)
    {
        this.onEventHit = onEventHit;
    }

    public void SetOnEventSend(Action onEventSend)
    {
        this.onEventSend = onEventSend;
    }

    public void SetFollowTarget(Transform target)
    {
        FollowTarget = target;
    }

    public void SetFlip(bool flip)
    {
        IsFlip = flip;
    }

    public void SetHitCount(int count)
    {
        HitCount = count;
    }

    public bool IsOverHitCount()
    {
        // 0인 경우에는 무제한 Hit
        if (HitCount == 0)
            return false;

        return currentHitCount >= HitCount;
    }

    public void OnEventHit(Collider2D collider)
    {
        if (IsOverHitCount())
            return;

        onEventHit(collider);
        currentHitCount++;
    }
}
