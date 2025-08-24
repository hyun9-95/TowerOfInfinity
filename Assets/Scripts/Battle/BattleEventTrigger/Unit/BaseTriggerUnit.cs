#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTriggerUnit<T> : PoolableBaseUnit<T>, IBattleEventTriggerUnit, IObserver where T : BattleEventTriggerUnitModel
{
    #region Value
    [SerializeField]
    protected IBattleEventTriggerUnit.ColliderDetectType detectType;

    [SerializeField]
    protected Collider2D hitCollider;

    [SerializeField]
    private bool hitFollowTargetOnly = false;

    protected Dictionary<CharacterUnitModel, float> nextAllowedTime = new();
    #endregion

    #region Lifecycle
    private void Awake()
    {
        hitCollider.enabled = false;
        OnUnitAwake();
    }

    protected virtual void OnUnitAwake()
    {

    }

    protected override void OnDisable()
    {
        OnUnitDisable();
        base.OnDisable();
    }

    protected virtual void OnUnitDisable()
    {
        TokenPool.Cancel(GetHashCode());
        hitCollider.enabled = false;

        RemoveEnemyKilledObserver();
        nextAllowedTime.Clear();
    }

    protected virtual async UniTask EnableColliderAsync(float detectStartTime, float detectDuration)
    {
        if (detectStartTime > 0)
            await UniTaskUtils.DelaySeconds(detectStartTime, TokenPool.Get(GetHashCode()));

        hitCollider.enabled = true;

        if (detectDuration == 0)
        {
            await UniTaskUtils.WaitForFixedUpdate(TokenPool.Get(GetHashCode()));
        }
        else
        {
            await UniTaskUtils.DelaySeconds(detectDuration, TokenPool.Get(GetHashCode()));
        }

        hitCollider.enabled = false;
    }

    protected async UniTask FollowAsync(Vector3 localPosOffset, float followTime)
    {
        float startTime = Time.time;

        while (!gameObject.CheckSafeNull() && gameObject.activeSelf)
        {
            if (followTime > 0 && Time.time - startTime >= followTime)
                break;

            transform.position = Model.FollowTargetTransform.transform.position;
            transform.localPosition += localPosOffset;
            await UniTaskUtils.WaitForFixedUpdate(TokenPool.Get(GetHashCode()));
        }
    }

    #endregion

    #region Trigger Detection
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Enter)
            return;

        OnDetectHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Stay)
            return;

        OnDetectHit(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Exit)
            return;

        OnDetectHit(other);
    }

    protected virtual void OnDetectHit(Collider2D other)
    {
        if (!other.gameObject.CheckLayer(LayerFlag.Character) || Model == null)
            return;

        // 팔로우 대상에게만 히트하는 경우
        if (hitFollowTargetOnly && Model.FollowTargetTransform != null && other.transform != Model.FollowTargetTransform)
            return;

        var targetModel = BattleApi.OnGetAliveCharModel(other);

        if (targetModel == null)
            return;

        float nowTime = Time.time;
        float coolTime = FloatDefine.COLLIDER_STAY_COOLTIME_PER_TARGET;

        if (nextAllowedTime != null && nextAllowedTime.TryGetValue(targetModel, out var time) && nowTime < time)
            return;

        Vector2 attackPos = hitCollider.transform.position;

        Model.OnEventHit(other, attackPos);
        
        if (nextAllowedTime == null)
            nextAllowedTime = new Dictionary<CharacterUnitModel, float>();
            
        nextAllowedTime[targetModel] = nowTime + coolTime;
    }
    #endregion

    #region Stay Cooldown
    protected void AddEnemyKilledObserver()
    {
        ObserverManager.AddObserver(BattleObserverID.DeadCharacter, this);
    }

    protected void RemoveEnemyKilledObserver()
    {
        ObserverManager.RemoveObserver(BattleObserverID.DeadCharacter, this);
    }

    private void RemoveTargetFromCooldown(CharacterUnitModel targetModel)
    {
        if (nextAllowedTime != null)
            nextAllowedTime.Remove(targetModel);
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam param)
            return;

        if (observerMessage is BattleObserverID.DeadCharacter)
            RemoveTargetFromCooldown(param.ModelValue);
    }
    #endregion
}