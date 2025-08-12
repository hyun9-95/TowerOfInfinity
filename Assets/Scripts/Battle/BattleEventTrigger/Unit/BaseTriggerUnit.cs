using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTriggerUnit<T> : PoolableBaseUnit<T>, IBattleEventTriggerUnit, IObserver where T : BattleEventTriggerUnitModel
{
    [SerializeField]
    protected IBattleEventTriggerUnit.ColliderDetectType detectType;

    [SerializeField]
    protected Collider2D hitCollider;

    protected Dictionary<CharacterUnitModel, float> nextAllowedTime;

    #region Lifecycle
    private void Awake()
    {
        hitCollider.enabled = false;
        OnUnitAwake();
    }

    protected virtual void OnUnitAwake() { }

    protected override void OnDisable()
    {
        OnUnitDisable();
        base.OnDisable();
    }

    protected virtual void OnUnitDisable()
    {
        TokenPool.Cancel(GetHashCode());
        hitCollider.enabled = false;
        
        if (detectType == IBattleEventTriggerUnit.ColliderDetectType.Stay)
        {
            OnStayDetectionDisable();
            if (nextAllowedTime != null)
                nextAllowedTime.Clear();
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

        OnTriggerStayDetected(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectType != IBattleEventTriggerUnit.ColliderDetectType.Exit)
            return;

        OnDetectHit(other);
    }

    protected virtual void OnDetectHit(Collider2D other)
    {
        if (!other.gameObject.CheckLayer(LayerFlag.Character))
            return;

        if (Model == null)
            return;

        Model.OnEventHit(other, transform.position);
    }

    protected virtual void OnTriggerStayDetected(Collider2D other)
    {
        OnDetectHitWithCooltime(other);
    }

    protected virtual void OnDetectHitWithCooltime(Collider2D other)
    {
        if (!other.gameObject.CheckLayer(LayerFlag.Character) || Model == null)
            return;

        var targetModel = BattleSceneManager.Instance.GetCharacterModel(other);

        if (targetModel == null)
            return;

        float now = Time.time;
        float cd = FloatDefine.COLLIDER_STAY_COOLTIME_PER_TARGET;

        if (nextAllowedTime != null && nextAllowedTime.TryGetValue(targetModel, out var t) && now < t)
            return;

        Model.OnEventHit(other, transform.position);
        
        if (nextAllowedTime == null)
            nextAllowedTime = new Dictionary<CharacterUnitModel, float>();
            
        nextAllowedTime[targetModel] = now + cd;
    }
    #endregion

    #region Stay Cooldown
    protected virtual void InitializeStayCooldown()
    {
        if (detectType == IBattleEventTriggerUnit.ColliderDetectType.Stay)
        {
            OnStayDetectionEnable();

            if (nextAllowedTime == null)
                nextAllowedTime = new Dictionary<CharacterUnitModel, float>();
        }
    }

    protected virtual void OnStayDetectionEnable()
    {
        ObserverManager.AddObserver(BattleObserverID.EnemyKilled, this);
    }

    protected virtual void OnStayDetectionDisable()
    {
        ObserverManager.RemoveObserver(BattleObserverID.EnemyKilled, this);
    }

    protected virtual void RemoveTargetFromCooldown(CharacterUnitModel targetModel)
    {
        if (detectType == IBattleEventTriggerUnit.ColliderDetectType.Stay)
        {
            if (nextAllowedTime != null && nextAllowedTime.ContainsKey(targetModel))
                nextAllowedTime.Remove(targetModel);
        }
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam param)
            return;

        RemoveTargetFromCooldown(param.ModelValue);
    }
    #endregion
}