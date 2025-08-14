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
        Cancel();
    }

    public void ReceiveBattleEvent(BattleEventModel model)
    {
        if (model == null)
            return;

        var sender = model.Sender;

        if (owner == null || sender == null)
            return;

        var battleEvent = factory.CreateNewBattleEvent(model.EventType);

        if (battleEvent == null)
            return;

        if (!battleEvent.CheckStartCondition(sender, owner, model))
            return;

        battleEvent.Initialize(model);

        // 지속시간이 없는 경우 바로 실행 => 종료
        if (battleEvent.Model.Duration == 0 && battleEvent.Model.Category != BattleEventCategory.Passive)
        {
            // 즉발인데 옵저버 사용하는 경우는 없어야함
            battleEvent.OnStart();
            battleEvent.OnEnd();
        }
        else
        {
            PendingBattleEvent(battleEvent);
        }
    }

    public void Update()
    {
        // 대기중인 효과들을 업데이트 목록에 추가
        ProcessPendingBattleEvents();

        // 업데이트 목록의 효과들 지속시간 갱신 및 처리
        UpdateAllBattleEvents();
    }

    private void ProcessPendingBattleEvents()
    {
        while (pendingEvents.Count > 0)
        {
            var battleEvent = pendingEvents.Dequeue();

            if (battleEvent == null)
                continue;

            // 업데이트 목록에 추가
            CategorizeEvent(battleEvent);
        }
    }

    private void UpdateAllBattleEvents()
    {
        tempUpdateList.Clear();
        tempRemoveList.Clear();

        tempUpdateList.AddRange(uniqueEventDic.Values);
        tempUpdateList.AddRange(stackableEventDic.Values);

        foreach (var battleEvent in tempUpdateList)
        {
            if (battleEvent == null)
                continue;

            UpdateBattleEvents(battleEvent, tempRemoveList);
        }

        // 종료된 효과 제거
        RemoveBattleEvents(tempRemoveList);
    }

    private void PendingBattleEvent(BattleEvent battleEvent)
    {
        if (battleEvent.IsStackable)
        {
            if (!stackableEventCountDic.ContainsKey(battleEvent.Model.EventType))
                stackableEventCountDic[battleEvent.Model.EventType] = 0;

            if (stackableEventDic.TryGetValue(battleEvent.Model.DataID, out BattleEvent existingEvent))
            {
                // 동일한 효과의 경우 지속시간 + 값 갱신
                existingEvent.OnReapply(battleEvent.Model);
            }
            else
            {
                EnqueueBattleEvent(battleEvent);
            }
        }
        else
        {
            if (uniqueEventDic.TryGetValue(battleEvent.Model.EventType, out BattleEvent existingEvent))
            {
                bool isEqual = existingEvent.Model.DataID == battleEvent.Model.DataID;

                if (isEqual)
                {
                    // 동일한 효과의 경우 지속시간 + 값 갱신
                    existingEvent.OnReapply(battleEvent.Model);
                    return;
                }
                else
                {
                    // 기존 효과가 있다면 종료 처리
                    existingEvent.OnEnd();
                    existingEvent.RemoveObserverIds();
                }
            }

            // 새로운 효과 적용
            EnqueueBattleEvent(battleEvent);
        }
    }

    private void EnqueueBattleEvent(BattleEvent battleEvent)
    {
        battleEvent.OnStart();

        if (battleEvent.Model.ApplyIntervalSeconds > 0)
            battleEvent.StartTriggerTime();

        battleEvent.AddObserverIds();
        pendingEvents.Enqueue(battleEvent);
    }

    private void CategorizeEvent(BattleEvent battleEvent)
    {
        if (battleEvent.IsStackable)
        {
            stackableEventDic[battleEvent.Model.DataID] = battleEvent;

            var statusType = battleEvent.Model.EventType;

            if (!stackableEventCountDic.ContainsKey(statusType))
                stackableEventCountDic[statusType] = 0;

            stackableEventCountDic[statusType]++;
        }
        else
        {
            uniqueEventDic[battleEvent.Model.EventType] = battleEvent;
        }

        if (battleEvent.Model.Group == BattleEventGroup.None)
            return;

        // 버프/디버프 그룹 분류
        var groupType = battleEvent.Model.Group;

        if (!eventGroupDic.ContainsKey(groupType))
            eventGroupDic[groupType] = new Dictionary<int, BattleEvent>();

        eventGroupDic[groupType][battleEvent.Model.DataID] = battleEvent;
    }

    private void UpdateBattleEvents(BattleEvent battleEvent, List<BattleEvent> eventToRemove)
    {
        // 패시브의 경우 지속시간이 없다.
        if (battleEvent.Model.Category == BattleEventCategory.Passive)
            return;

        battleEvent.ReduceDuration();

        if (battleEvent.CheckEndCondition())
        {
            eventToRemove.Add(battleEvent);
        }
        else
        {
            battleEvent.OnUpdate();
        }
    }

    private void RemoveBattleEvents(List<BattleEvent> eventToRemove)
    {
        foreach (var battleEvent in eventToRemove)
        {
            if (battleEvent == null)
                continue;

            battleEvent.OnEnd();
            battleEvent.RemoveObserverIds();

            var statusType = battleEvent.Model.EventType;

            if (battleEvent.IsStackable)
            {
                stackableEventDic.Remove(battleEvent.Model.DataID);

                if (stackableEventCountDic.ContainsKey(battleEvent.Model.EventType))
                {
                    stackableEventCountDic[battleEvent.Model.EventType] =
                        Mathf.Max(0, stackableEventCountDic[battleEvent.Model.EventType] - 1);
                }
            }
            else
            {
                uniqueEventDic.Remove(battleEvent.Model.EventType);
            }

            // 버프/디버프 그룹에서 제거
            if (battleEvent.Model.Group == BattleEventGroup.None)
                continue;

            if (eventGroupDic.TryGetValue(battleEvent.Model.Group, out var groupDic))
                groupDic.Remove(battleEvent.Model.DataID);
        }

        if (eventToRemove.Count > 0)
            eventToRemove.Clear();
    }

    private void StopAllBattleEvent()
    {
        List<BattleEvent> eventToStopList = new List<BattleEvent>();

        eventToStopList.AddRange(uniqueEventDic.Values);
        eventToStopList.AddRange(stackableEventDic.Values);

        foreach (var battleEvent in eventToStopList)
        {
            if (battleEvent == null)
                continue;

            battleEvent.OnEnd();
            battleEvent.RemoveObserverIds();
        }
    }

    public void Cancel()
    {
        // 먼저 모든 상태 효과를 종료 처리
        StopAllBattleEvent();

        pendingEvents.Clear();
        uniqueEventDic.Clear();
        stackableEventCountDic.Clear();
        stackableEventDic.Clear();
        eventGroupDic.Clear();
    }

    #region Util
    public bool IsProcessingBattleEvent(BattleEventType statusType)
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

    public IEnumerable<BattleEvent> GetBattleEventsByEffectGroup(BattleEventGroup effectGroupType)
    {
        if (eventGroupDic.TryGetValue(effectGroupType, out var groupDic))
            return groupDic.Values;

        return null;
    }
    #endregion
    #endregion
}
