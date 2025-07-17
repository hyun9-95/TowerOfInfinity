using System.Collections.Generic;
using UnityEngine;

public class BattleEventProcessor
{
    #region Property
    #endregion

    #region Value
    private BattleEventFactory factory = new BattleEventFactory();

    private CharacterUnitModel owner;

    // 업데이트 목록에 추가될 효과들
    private Queue<BattleEvent> pendingEvents = new Queue<BattleEvent>();

    // 중첩 불가 단일 효과
    private Dictionary<BattleEventType , BattleEvent> uniqueEventDic = new Dictionary<BattleEventType, BattleEvent>();

    // 중첩 가능한 효과
    private Dictionary<int, BattleEvent> stackableEventDic = new Dictionary<int, BattleEvent>();
    private Dictionary<BattleEventType, int> stackableEventCountDic = new Dictionary<BattleEventType, int>();

    // 버프/디버프 그룹
    private Dictionary<BattleEventGroup, Dictionary<int, BattleEvent>> eventGroupDic = new Dictionary<BattleEventGroup, Dictionary<int, BattleEvent>>();

    // 한프레임 돌고 Clear되는 리스트
    private List<BattleEvent> tempUpdateList = new List<BattleEvent>();
    private List<BattleEvent> tempRemoveList = new List<BattleEvent>();
    #endregion

    #region Function

    public void SetOwner(CharacterUnitModel ownerValue)
    {
        owner = ownerValue;

        // 오너 변경시 클리어
        Clear();
    }

    public void SendBattleEvent(BattleEventModel model)
    {
        if (model == null)
            return;

        var sender = model.Sender;

        if (owner == null || sender == null)
            return;

        var battleStatusEffect = factory.CreateNewBattleStatusEffect(model.EventType);

        if (battleStatusEffect == null)
            return;

        if (!battleStatusEffect.CheckStartCondition(sender, owner, model))
            return;

        battleStatusEffect.Initialize(model);

        // 지속시간이 없는 경우 바로 실행 => 종료
        if (battleStatusEffect.Model.Duration == 0 && battleStatusEffect.Model.Category != BattleEventCategory.Passive)
        {
            // 즉발인데 옵저버 사용하는 경우는 없어야함
            battleStatusEffect.OnStart();
            battleStatusEffect.OnUpdate();
            battleStatusEffect.OnEnd();
        }
        else
        {
            PendingStatusEffect(battleStatusEffect);
        }
    }

    public void Update()
    {
        // 대기중인 효과들을 업데이트 목록에 추가
        ProcessPendingBattleStatusEffects();

        // 업데이트 목록의 효과들 지속시간 갱신 및 처리
        UpdateAllBattleStatusEffects();
    }

    private void ProcessPendingBattleStatusEffects()
    {
        while (pendingEvents.Count > 0)
        {
            var battleStatusEffect = pendingEvents.Dequeue();

            if (battleStatusEffect == null)
                continue;

            // 업데이트 목록에 추가
            AddToStatusEffectDic(battleStatusEffect);
        }
    }

    private void UpdateAllBattleStatusEffects()
    {
        tempUpdateList.Clear();
        tempRemoveList.Clear();

        tempUpdateList.AddRange(uniqueEventDic.Values);
        tempUpdateList.AddRange(stackableEventDic.Values);

        foreach (var statusEffect in tempUpdateList)
        {
            if (statusEffect == null)
                continue;

            UpdateBattleStatusEffect(statusEffect, tempRemoveList);
        }

        // 종료된 효과 제거
        RemoveBattleStatusEffects(tempRemoveList);
    }

    private void PendingStatusEffect(BattleEvent battleStatusEffect)
    {
        if (battleStatusEffect.IsStackable)
        {
            if (!stackableEventCountDic.ContainsKey(battleStatusEffect.Model.EventType))
                stackableEventCountDic[battleStatusEffect.Model.EventType] = 0;

            if (stackableEventDic.TryGetValue(battleStatusEffect.Model.DataID, out BattleEvent existingStatusEffect))
            {
                // 동일한 효과의 경우 지속시간만 갱신
                existingStatusEffect.SetRemainDuration(battleStatusEffect.Model.Duration);
            }
            else
            {
                EnqueueBattleStatusEffect(battleStatusEffect);
            }
        }
        else
        {
            if (uniqueEventDic.TryGetValue(battleStatusEffect.Model.EventType, out BattleEvent existingStatusEffect))
            {
                bool isEqual = existingStatusEffect.Model.DataID == battleStatusEffect.Model.DataID;

                if (isEqual)
                {
                    // 동일한 효과의 경우 지속시간만 갱신
                    existingStatusEffect.SetRemainDuration(battleStatusEffect.Model.Duration);
                    return;
                }
                else
                {
                    // 기존 효과가 있다면 종료 처리
                    existingStatusEffect.OnEnd();
                    existingStatusEffect.RemoveObserverIds();
                }
            }

            // 새로운 효과 적용
            EnqueueBattleStatusEffect(battleStatusEffect);
        }
    }

    private void EnqueueBattleStatusEffect(BattleEvent battleStatusEffect)
    {
        battleStatusEffect.OnStart();
        battleStatusEffect.AddObserverIds();
        pendingEvents.Enqueue(battleStatusEffect);
    }

    private void AddToStatusEffectDic(BattleEvent battleStatusEffect)
    {
        if (battleStatusEffect.IsStackable)
        {
            stackableEventDic[battleStatusEffect.Model.DataID] = battleStatusEffect;

            var statusType = battleStatusEffect.Model.EventType;

            if (!stackableEventCountDic.ContainsKey(statusType))
                stackableEventCountDic[statusType] = 0;

            stackableEventCountDic[statusType]++;
        }
        else
        {
            uniqueEventDic[battleStatusEffect.Model.EventType] = battleStatusEffect;
        }

        if (battleStatusEffect.Model.Group == BattleEventGroup.None)
            return;

        // 버프/디버프 그룹 분류
        var groupType = battleStatusEffect.Model.Group;

        if (!eventGroupDic.ContainsKey(groupType))
            eventGroupDic[groupType] = new Dictionary<int, BattleEvent>();

        eventGroupDic[groupType][battleStatusEffect.Model.DataID] = battleStatusEffect;
    }

    private void UpdateBattleStatusEffect(BattleEvent battleStatusEffect, List<BattleEvent> statusEffectToRemove)
    {
        // 패시브의 경우 지속시간이 없다.
        if (battleStatusEffect.Model.Category == BattleEventCategory.Passive)
            return;

        battleStatusEffect.ReduceDuration();

        if (battleStatusEffect.CheckEndCondition())
        {
            statusEffectToRemove.Add(battleStatusEffect);
        }
        else
        {
            battleStatusEffect.OnUpdate();
        }
    }

    private void RemoveBattleStatusEffects(List<BattleEvent> statusEffectToRemove)
    {
        foreach (var statusEffect in statusEffectToRemove)
        {
            if (statusEffect == null)
                continue;

            statusEffect.OnEnd();
            statusEffect.RemoveObserverIds();

            var statusType = statusEffect.Model.EventType;

            if (statusEffect.IsStackable)
            {
                stackableEventDic.Remove(statusEffect.Model.DataID);

                if (stackableEventCountDic.ContainsKey(statusEffect.Model.EventType))
                {
                    stackableEventCountDic[statusEffect.Model.EventType] =
                        Mathf.Max(0, stackableEventCountDic[statusEffect.Model.EventType] - 1);
                }
            }
            else
            {
                uniqueEventDic.Remove(statusEffect.Model.EventType);
            }

            // 버프/디버프 그룹에서 제거
            if (statusEffect.Model.Group == BattleEventGroup.None)
                continue;

            if (eventGroupDic.TryGetValue(statusEffect.Model.Group, out var groupDic))
                groupDic.Remove(statusEffect.Model.DataID);
        }

        if (statusEffectToRemove.Count > 0)
            statusEffectToRemove.Clear();
    }

    public void Clear()
    {
        pendingEvents.Clear();
        uniqueEventDic.Clear();
        stackableEventCountDic.Clear();
        stackableEventDic.Clear();
        eventGroupDic.Clear();
    }

    #region Util
    public bool IsBattleStatusEffect(BattleEventType statusType)
    {
        if (uniqueEventDic.ContainsKey(statusType))
            return true;

        if (stackableEventCountDic.ContainsKey(statusType))
            return true;

        return false;
    }

    public int GetStackableStatusCount(BattleEventType statusType)
    {
        if (stackableEventCountDic.TryGetValue(statusType, out int count))
            return count;

        if (uniqueEventDic.ContainsKey(statusType))
            return 1;

        return 0;
    }

    public IEnumerable<BattleEvent> GetStatusEffectsByEffectGroup(BattleEventGroup effectGroupType)
    {
        if (eventGroupDic.TryGetValue(effectGroupType, out var groupDic))
            return groupDic.Values;

        return null;
    }
    #endregion
    #endregion
}
