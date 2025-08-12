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
    protected float fadeTime = 0f;

    [SerializeField]
    protected float fadeInTime = 0;

    [SerializeField]
    protected Collider2D hitCollider;

    protected Dictionary<CharacterUnitModel, float> nextAllowedTime = new();
    #endregion

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

    public override async UniTask ShowAsync()
    {
        AddEnemyKilledObserver();
    }

    protected virtual void OnUnitDisable()
    {
        TokenPool.Cancel(GetHashCode());
        hitCollider.enabled = false;

        RemoveEnemyKilledObserver();
        nextAllowedTime.Clear();
    }

    protected void DeactivateWithFade()
    {
        Deactivate(fadeTime);
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

        var targetModel = BattleSceneManager.Instance.GetCharacterModel(other);

        if (targetModel == null)
            return;

        float nowTime = Time.time;
        float coolTime = FloatDefine.COLLIDER_STAY_COOLTIME_PER_TARGET;

        if (nextAllowedTime != null && nextAllowedTime.TryGetValue(targetModel, out var time) && nowTime < time)
            return;

        Model.OnEventHit(other, hitCollider.transform.position);
        
        if (nextAllowedTime == null)
            nextAllowedTime = new Dictionary<CharacterUnitModel, float>();
            
        nextAllowedTime[targetModel] = nowTime + coolTime;
    }
    #endregion

    #region Stay Cooldown
    protected void AddEnemyKilledObserver()
    {
        ObserverManager.AddObserver(BattleObserverID.EnemyKilled, this);
    }

    protected void RemoveEnemyKilledObserver()
    {
        ObserverManager.RemoveObserver(BattleObserverID.EnemyKilled, this);
    }

    private void RemoveTargetFromCooldown(CharacterUnitModel targetModel)
    {
        if (nextAllowedTime != null && nextAllowedTime.ContainsKey(targetModel))
            nextAllowedTime.Remove(targetModel);
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        if (observerParam is not BattleObserverParam param)
            return;

        RemoveTargetFromCooldown(param.ModelValue);
    }
    #endregion
}