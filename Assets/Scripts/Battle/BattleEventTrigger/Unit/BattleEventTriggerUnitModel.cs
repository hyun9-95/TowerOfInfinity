using System;
using UnityEngine;

public class BattleEventTriggerUnitModel : IBaseUnitModel
{
    #region Property
    public CharacterUnitModel FollowTargetModel { get; private set; }
    public Transform FollowTargetTransform => FollowTargetModel.Transform;

    public bool IsEnableFollow => FollowTargetModel != null && FollowTargetModel.IsActivated && FollowTargetModel.Transform != null;

    public bool IsFlip { get; private set; }

    public TeamTag DetectTeamTag { get; private set; }

    public int HitCount { get; private set; }

    public Vector2 StartDirection { get; private set; }
    #endregion

    #region Value
    public int currentHitCount;

    /// <summary>
    /// 유닛에서 타겟을 검출할 때.
    /// </summary>
    private Func<Collider2D, Vector3, bool> onEventHit;
    #endregion

    public void SetDetectTeamTag(TeamTag detectTeamTag)
    {
        DetectTeamTag = detectTeamTag;
    }

    public void SetOnEventHit(Func<Collider2D, Vector3, bool> onEventHit)
    {
        this.onEventHit = onEventHit;
    }

    public void SetFollowTargetModel(CharacterUnitModel target)
    {
        FollowTargetModel = target;
    }

    public void SetFlip(bool flip)
    {
        IsFlip = flip;
    }

    public void SetHitCount(int count)
    {
        HitCount = count;
    }

    public void SetDirection(Vector3 direction)
    {
        StartDirection = direction;
    }

    public bool IsOverHitCount()
    {
        // 0인 경우에는 무제한 Hit
        if (HitCount == 0)
            return false;

        return currentHitCount >= HitCount;
    }

    public void OnEventHit(Collider2D collider, Vector3 hitPos)
    {
        if (IsOverHitCount())
            return;

        bool hitResult = onEventHit(collider, hitPos);

        if (hitResult)
            currentHitCount++;
    }
}
