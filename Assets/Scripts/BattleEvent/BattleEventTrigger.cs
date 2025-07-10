#pragma warning disable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class BattleEventTrigger
{
    protected virtual bool UseTrigger => true;
    protected BattleEventTriggerModel Model { get; private set; }

    protected int currentTargetCount = 0;

    protected Dictionary<CharacterUnitModel, int> targetHitCount;

    public void SetModel(BattleEventTriggerModel skillInfoValue)
    {
        Model = skillInfoValue;
    }

    public void Reset()
    {
        currentTargetCount = 0;

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
        if (IsOverTargetCount(currentTargetCount))
            return;

        if (hitTarget == null)
            return;

        var model = GetTargetModel(hitTarget);

        if (model == null)
            return;

        if (model.TeamTag == Model.Sender.TeamTag)
            return;

        switch (Model.TargetType)
        {
            case BattleEventTargetType.Single:
            case BattleEventTargetType.Multiple:
                if (UseTrigger)
                {
                    ProcessBattleEvent(model, hitTarget);
                }
                else
                {
                    ProcessNoneTriggerBattleEvent(model, hitTarget);
                }   
                break;
               
            default:
                Logger.Error($"이 타입으로 사용하면 안되는 타겟타입 : {Model.TargetType}");
                return;
        }
    }

    private void ProcessBattleEvent(CharacterUnitModel targetModel, Collider2D hitTarget)
    {
        SendBattleEvent(targetModel);

        if (!string.IsNullOrEmpty(Model.HitEffectPrefabName))
            OnSpawnHitEffect(hitTarget.transform.position).Forget();

        currentTargetCount++;

        if (IsOverTargetCount(currentTargetCount))
            OnComplete();
    }

    private void ProcessNoneTriggerBattleEvent(CharacterUnitModel targetModel, Collider2D hitTarget)
    {
        if (targetHitCount == null)
            targetHitCount = new Dictionary<CharacterUnitModel, int>();

        if (!targetHitCount.ContainsKey(targetModel))
        {
            ProcessBattleEvent(targetModel, hitTarget);
            targetHitCount[targetModel] = 1;
            return;
        }

        int hitCount = targetHitCount[targetModel];

        if (IsOverTargetCount(hitCount))
            return;

        ProcessBattleEvent(targetModel, hitTarget);
        targetHitCount[targetModel]++;
    }

    protected bool IsOverTargetCount(int count)
    {
        return count >= Model.TargetCount;
    }

    protected bool IsOverTargetCount()
    {
        return currentTargetCount >= Model.TargetCount;
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
        var battleEvent = Model.CreateBattleEvent(target);

        if (battleEvent != null)
            target.EnqueueBattleEvent(battleEvent);
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
