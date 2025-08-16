using System;
using UnityEngine;

public abstract class BattleEvent : IObserver
{
    public bool IsTimeOver => RemainDuration <= 0;
    public BattleEventModel Model { get; private set; }
    public float RemainDuration { get; protected set; }
    public float LastTriggerTime { get; protected set; }

    /// <summary>
    /// 중첩 가능 여부
    /// </summary>
    public bool IsStackable => Model.Stackable;

    public virtual bool CheckStartCondition(CharacterUnitModel sender, CharacterUnitModel recevier, BattleEventModel statusEffect)
    {
        return !recevier.IsDead;
    }

    public virtual bool CheckEndCondition()
    {
        if (Model.Receiver == null || Model.Receiver.IsDead)
            return true;

        if (IsTimeOver)
            return true;

        return false;
    }

    protected virtual BattleObserverID[] UseObserverIds => null;

    public void Initialize(BattleEventModel eventModel)
    {
        Model = eventModel;
        RemainDuration = eventModel.Duration;
    }

    public void ReduceDuration()
    {
        RemainDuration -= Time.deltaTime;
    }

    public void SetRemainDuration(float value)
    {
        RemainDuration = value;
    }

    public void AddObserverIds()
    {
        if (UseObserverIds == null)
            return;

        foreach (var observerId in UseObserverIds)
            ObserverManager.AddObserver(observerId, this);
    }

    public virtual void OnStart()
    {
    }

    public virtual void StartTriggerTime()
    {
        LastTriggerTime = Time.time;
    }

    // 매 프레임 처리 필요한 경우 구현
    public virtual void OnUpdate()
    {
        if (Model == null)
            return;

        if (Model.ApplyIntervalSeconds == 0)
            return;

        if (Time.time - LastTriggerTime >= Model.ApplyIntervalSeconds)
        {
            LastTriggerTime += Model.ApplyIntervalSeconds;
            OnInterval();
        }
    }

    // 일정 주기마다 호출되는 함수
    public virtual void OnInterval()
    {
    }

    protected virtual float GetAppliableStatValue()
    {
        if (Model == null)
            return 0;

        if (Model.ValueApplyType == ValueApplyType.None)
            return 0;

        var statReferenceTarget = GetStatReferenceTarget();

        if (statReferenceTarget == null)
            return 0;

        float finalValue = Model.Value;

        if (Model.StatReference != StatReference.None)
        {
            var baseStat = statReferenceTarget.GetStatValue(Model.AffectStat, Model.StatReferenceCondition);
            
            if (Model.ValueApplyType == ValueApplyType.Mutiply)
                finalValue = baseStat * Model.Value;

            if (Model.ValueApplyType == ValueApplyType.Add)
                finalValue = baseStat + Model.Value;
        }

        var sign = Model.StatusDirection is StatusDirection.Increase ? 1 : -1;

        return finalValue * sign;
    }

    public virtual void OnEnd()
    {
    }

    public void RemoveObserverIds()
    {
        if (UseObserverIds == null)
            return;

        foreach (var observerId in UseObserverIds)
            ObserverManager.RemoveObserver(observerId, this);
    }

    // 옵저버 수신 처리
    protected virtual void OnHandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
    }

    void IObserver.HandleMessage(Enum observerMessage, IObserverParam observerParam)
    {
        OnHandleMessage(observerMessage, observerParam);
    }

    #region Util
    public CharacterUnitModel GetStatReferenceTarget()
    {
        if (Model == null)
            return null;

        if (Model.StatReference == StatReference.Sender)
            return Model.Sender;

        return Model.Receiver;
    }

    public void OnReapply(BattleEventModel model)
    {
        if (model == null || Model == null)
            return;

        Model.UpdateValue(model);
        RemainDuration = model.Duration;
    }
    #endregion
}
