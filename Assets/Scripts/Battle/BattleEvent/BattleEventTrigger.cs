#pragma warning disable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEventTrigger
{
   protected BattleEventTriggerModel Model { get; private set; }

    protected int currentHitCount = 0;

    protected Dictionary<CharacterUnitModel, int> targetHitCount;

    public void SetModel(BattleEventTriggerModel skillInfoValue)
    {
        Model = skillInfoValue;
    }

    public void Reset()
    {
        currentHitCount = 0;

        if (targetHitCount != null)
            targetHitCount.Clear();

        Model = null;
    }

    protected List<CharacterUnitModel> OnFindMultipleTargets(Collider2D[] colliders)
    {
        if (colliders == null || colliders.Length == 0)
            return null;

        List<CharacterUnitModel> targets = new List<CharacterUnitModel>();

        switch (Model.TargetType)
        {
            case BattleEventTargetType.Multiple:
                // 모든 타겟 추가
                foreach (var collider in colliders)
                {
                    if (collider == null)
                        continue;

                    var targetModel = GetTargetModel(collider);

                    if (targetModel != null)
                        targets.Add(targetModel);
                }
                break;

            default:
                Logger.Log($"이 타입은 여기서 체크하면 안된다. {Model.TargetType}");
                return null;
        }

        return targets;
    }

    protected CharacterUnitModel OnFindSingleTarget(Collider2D collider)
    {
        if (collider == null || Model.TargetType != BattleEventTargetType.Single)
            return null;

        return GetTargetModel(collider);
    }

    protected CharacterUnitModel GetTargetModel(Collider2D collider)
    {
        var targetModel = BattleSceneManager.Instance.GetCharacterModel(collider.gameObject.GetInstanceID());

        // 죽으면 타겟으로 감지안함.
        if (targetModel != null && targetModel.Hp <= 0)
            return null;

        return targetModel;
    }

    public virtual async UniTask Process() { }

    protected void OnEventHit(Collider2D hitTarget)
    {
        if (IsOverHitCount(currentHitCount))
            return;

        if (hitTarget == null)
            return;

        var model = GetTargetModel(hitTarget);

        if (model == null)
            return;

        if (model.TeamTag == Model.Sender.TeamTag)
            return;

        switch (Model.HitCountingType)
        {
            // 단순 HitCount로 계산
            case HitCountingType.Total:
                ProcessTotalHits(model, hitTarget);
                break;

            // 타겟별로 히트카운트를 따로 계산
            case HitCountingType.PerTarget:
                ProcessHitsPerTarget(model, hitTarget);
                break;
        }
    }

    private void ProcessTotalHits(CharacterUnitModel targetModel, Collider2D hitTarget)
    {
        if (Model.BattleEventDatas.Count == 1)
        {
            SendBattleEvent(targetModel);
        }
        else
        {
            SendBattleEvents(targetModel);
        }

        if (!string.IsNullOrEmpty(Model.HitEffectPrefabName))
            OnSpawnHitEffect(hitTarget.transform.position).Forget();

        currentHitCount++;

        if (IsOverHitCount(currentHitCount))
            OnComplete();
    }

    private void ProcessHitsPerTarget(CharacterUnitModel targetModel, Collider2D hitTarget)
    {
        if (targetHitCount == null)
            targetHitCount = new Dictionary<CharacterUnitModel, int>();

        if (!targetHitCount.ContainsKey(targetModel))
        {
            ProcessTotalHits(targetModel, hitTarget);
            targetHitCount[targetModel] = 1;
            return;
        }

        int hitCount = targetHitCount[targetModel];

        if (IsOverHitCount(hitCount))
            return;

        ProcessTotalHits(targetModel, hitTarget);
        targetHitCount[targetModel]++;
    }

    protected bool IsOverHitCount(int count)
    {
        return count >= Model.HitCount;
    }

    protected bool IsOverTargetCount()
    {
        return currentHitCount >= Model.HitCount;
    }

    protected async UniTask OnSpawnHitEffect(Vector3 pos)
    {
        var hitEffect = await ObjectPoolManager.Instance.SpawnTimedMono<TimedPoolableMono>
            (Model.HitEffectPrefabName, position: pos, Quaternion.identity);

        hitEffect.transform.localPosition += hitEffect.LocalPosOffset;
        hitEffect.Flip(hitEffect.transform.position.x < Model.Sender.Transform.position.x);
    }

    protected void SendBattleEvent(CharacterUnitModel target)
    {
        var battleEventModel = Model.CreateBattleEventModel(target);
        target.EventProcessorWrapper.SendBattleEvent(battleEventModel);
    }

    protected void SendBattleEvents(CharacterUnitModel target)
    {
        var battleEventModels = Model.CreateBattleEventModels(target);
        foreach (var battleEventModel in battleEventModels)
        {
            target.EventProcessorWrapper.SendBattleEvent(battleEventModel);
        }
    }

    protected Vector2 OnGetFixedDirection(DirectionType directionType)
    {
        Vector2 direction = directionType switch
        {
            DirectionType.Up => Vector2.up,
            DirectionType.Down => Vector2.down,
            DirectionType.Left => Vector2.left,
            DirectionType.Right => Vector2.right,
            DirectionType.Owner => Model.Sender.IsFlipX ? Vector2.left : Vector2.right,
            _ => Vector2.zero
        };

        return direction;
    }

    protected virtual void OnComplete()
    {
        
    }

    public virtual void ReturnToPool()
    {
        BattleEventTriggerFactory.ReturnToPool(Model);
        BattleEventTriggerFactory.ReturnToPool(this);
    }
}
