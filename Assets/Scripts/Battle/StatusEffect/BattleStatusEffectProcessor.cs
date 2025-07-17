using System.Collections.Generic;
using UnityEngine;

public class BattleStatusEffectProcessor
{
    #region Property
    #endregion

    #region Value
    private BattleStatusEffectDefine typeDefine = new BattleStatusEffectDefine();

    private CharacterUnitModel owner;

    // 업데이트 목록에 추가될 효과들
    private Queue<BattleStatusEffect> pendingBattleStatusEffects = new Queue<BattleStatusEffect>();

    // 중첩 불가 단일 효과
    private Dictionary<BattleEventType , BattleStatusEffect> uniqueBattleStatusEffectDic = new Dictionary<BattleEventType, BattleStatusEffect>();

    // 중첩 가능한 효과
    private Dictionary<int, BattleStatusEffect> stackableStatusEffectDic = new Dictionary<int, BattleStatusEffect>();
    private Dictionary<BattleEventType, int> stackableEffectsCountDic = new Dictionary<BattleEventType, int>();

    // 버프/디버프 그룹
    private Dictionary<BattleEventGroup, Dictionary<int, BattleStatusEffect>> statusEffectGroupDic = new Dictionary<BattleEventGroup, Dictionary<int, BattleStatusEffect>>();

    // 한프레임 돌고 Clear되는 리스트
    private List<BattleStatusEffect> tempUpdateList = new List<BattleStatusEffect>();
    private List<BattleStatusEffect> tempRemoveList = new List<BattleStatusEffect>();
    #endregion

    #region Function
#if UNITY_EDITOR
    public Queue<BattleStatusEffect> EditorPendingBattleStatusEffects { get { return pendingBattleStatusEffects; } }
    public Dictionary<BattleEventType, BattleStatusEffect> EditorUniqueBattleStatusEffectDic { get { return uniqueBattleStatusEffectDic; } }
    public Dictionary<int, BattleStatusEffect> EditorStackableStatusEffectDic { get { return stackableStatusEffectDic; } }
    public Dictionary<BattleEventGroup, Dictionary<int, BattleStatusEffect>> EditorStatusEffectGroupDic { get { return statusEffectGroupDic; } }
#endif

    public void SetOwner(CharacterUnitModel ownerValue)
    {
        owner = ownerValue;

        // 오너 변경시 클리어
        Clear();
    }

    public void SendBattleStatusEffect(CharacterUnitModel sender, BattleEventModel model)
    {
        if (owner == null || sender == null)
            return;

        var battleStatusEffect = typeDefine.CreateNewBattleStatusEffect(model.BattleEventType);

        if (battleStatusEffect == null)
            return;

        if (!battleStatusEffect.CheckStartCondition(sender, owner, model))
            return;

        battleStatusEffect.Initialize(sender, owner, model);

        // 지속시간이 없는 경우 바로 실행 => 종료
        if (battleStatusEffect.Model.Duration == 0 && battleStatusEffect.Model.EffectCategoryType != BattleEventCategory.Passive)
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

    /// <summary>
    /// ControlsScript에서 매프레임 호출
    /// </summary>
    public void Update()
    {
        // 대기중인 효과들을 업데이트 목록에 추가
        ProcessPendingBattleStatusEffects();

        // 업데이트 목록의 효과들 지속시간 갱신 및 처리
        UpdateAllBattleStatusEffects();
    }

    private void ProcessPendingBattleStatusEffects()
    {
        while (pendingBattleStatusEffects.Count > 0)
        {
            var battleStatusEffect = pendingBattleStatusEffects.Dequeue();

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

        tempUpdateList.AddRange(uniqueBattleStatusEffectDic.Values);
        tempUpdateList.AddRange(stackableStatusEffectDic.Values);

        foreach (var statusEffect in tempUpdateList)
        {
            if (statusEffect == null)
                continue;

            UpdateBattleStatusEffect(statusEffect, tempRemoveList);
        }

        // 종료된 효과 제거
        RemoveBattleStatusEffects(tempRemoveList);
    }

    private void PendingStatusEffect(BattleStatusEffect battleStatusEffect)
    {
        if (battleStatusEffect.IsStackable)
        {
            if (!stackableEffectsCountDic.ContainsKey(battleStatusEffect.Model.BattleEventType))
                stackableEffectsCountDic[battleStatusEffect.Model.BattleEventType] = 0;

            if (stackableStatusEffectDic.TryGetValue(battleStatusEffect.Model.DataID, out BattleStatusEffect existingStatusEffect))
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
            if (uniqueBattleStatusEffectDic.TryGetValue(battleStatusEffect.Model.BattleEventType, out BattleStatusEffect existingStatusEffect))
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

    private void EnqueueBattleStatusEffect(BattleStatusEffect battleStatusEffect)
    {
        battleStatusEffect.OnStart();
        battleStatusEffect.AddObserverIds();
        pendingBattleStatusEffects.Enqueue(battleStatusEffect);
    }

    private void AddToStatusEffectDic(BattleStatusEffect battleStatusEffect)
    {
        if (battleStatusEffect.IsStackable)
        {
            stackableStatusEffectDic[battleStatusEffect.Model.DataID] = battleStatusEffect;

            var statusType = battleStatusEffect.Model.BattleEventType;

            if (!stackableEffectsCountDic.ContainsKey(statusType))
                stackableEffectsCountDic[statusType] = 0;

            stackableEffectsCountDic[statusType]++;
        }
        else
        {
            uniqueBattleStatusEffectDic[battleStatusEffect.Model.BattleEventType] = battleStatusEffect;
        }

        if (battleStatusEffect.Model.EffectGroupType == BattleEventGroup.None)
            return;

        // 버프/디버프 그룹 분류
        var groupType = battleStatusEffect.Model.EffectGroupType;

        if (!statusEffectGroupDic.ContainsKey(groupType))
            statusEffectGroupDic[groupType] = new Dictionary<int, BattleStatusEffect>();

        statusEffectGroupDic[groupType][battleStatusEffect.Model.DataID] = battleStatusEffect;
    }

    private void UpdateBattleStatusEffect(BattleStatusEffect battleStatusEffect, List<BattleStatusEffect> statusEffectToRemove)
    {
        // 패시브의 경우 지속시간이 없다.
        if (battleStatusEffect.Model.EffectCategoryType == BattleEventCategory.Passive)
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

    private void RemoveBattleStatusEffects(List<BattleStatusEffect> statusEffectToRemove)
    {
        foreach (var statusEffect in statusEffectToRemove)
        {
            if (statusEffect == null)
                continue;

            statusEffect.OnEnd();
            statusEffect.RemoveObserverIds();

            var statusType = statusEffect.Model.BattleEventType;

            if (statusEffect.IsStackable)
            {
                stackableStatusEffectDic.Remove(statusEffect.Model.DataID);

                if (stackableEffectsCountDic.ContainsKey(statusEffect.Model.BattleEventType))
                {
                    stackableEffectsCountDic[statusEffect.Model.BattleEventType] =
                        Mathf.Max(0, stackableEffectsCountDic[statusEffect.Model.BattleEventType] - 1);
                }
            }
            else
            {
                uniqueBattleStatusEffectDic.Remove(statusEffect.Model.BattleEventType);
            }

            // 버프/디버프 그룹에서 제거
            if (statusEffect.Model.EffectGroupType == BattleEventGroup.None)
                continue;

            if (statusEffectGroupDic.TryGetValue(statusEffect.Model.EffectGroupType, out var groupDic))
                groupDic.Remove(statusEffect.Model.DataID);
        }

        if (statusEffectToRemove.Count > 0)
            statusEffectToRemove.Clear();
    }

    public void Clear()
    {
        pendingBattleStatusEffects.Clear();
        uniqueBattleStatusEffectDic.Clear();
        stackableEffectsCountDic.Clear();
        stackableStatusEffectDic.Clear();
        statusEffectGroupDic.Clear();
    }

    #region Util
    public bool IsBattleStatusEffect(BattleEventType statusType)
    {
        if (uniqueBattleStatusEffectDic.ContainsKey(statusType))
            return true;

        if (stackableEffectsCountDic.ContainsKey(statusType))
            return true;

        return false;
    }

    public int GetStackableStatusCount(BattleEventType statusType)
    {
        if (stackableEffectsCountDic.TryGetValue(statusType, out int count))
            return count;

        if (uniqueBattleStatusEffectDic.ContainsKey(statusType))
            return 1;

        return 0;
    }

    public IEnumerable<BattleStatusEffect> GetStatusEffectsByEffectGroup(BattleEventGroup effectGroupType)
    {
        if (statusEffectGroupDic.TryGetValue(effectGroupType, out var groupDic))
            return groupDic.Values;

        return null;
    }
    #endregion
    #endregion
}
