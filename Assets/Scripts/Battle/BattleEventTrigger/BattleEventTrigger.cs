#pragma warning disable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEventTrigger
{
    protected BattleEventTriggerModel Model { get; private set; }
    protected List<IBattleEventTriggerUnit> activeUnits = new List<IBattleEventTriggerUnit>();

    protected int currentHitCount = 0;
 
    public void SetModel(BattleEventTriggerModel skillInfoValue)
    {
        Model = skillInfoValue;
    }

    public void Reset()
    {
        currentHitCount = 0;
        Model = null;
        activeUnits.Clear();
    }

    protected async UniTask<T> SpawnUnitAsync<T>(string prefabName, Vector3 position, Quaternion rotation) where T : PoolableMono, IBattleEventTriggerUnit
    {
        var unit = await ObjectPoolManager.Instance.SpawnPoolableMono<T>(prefabName, position, rotation);
        
        if (unit != null)
            activeUnits.Add(unit);

        return unit;
    }

    protected void DeactivateAllUnits()
    {
        foreach (var unit in activeUnits)
        {
            // 켜져있는것만.. 이미 꺼져있는건 ObjectPool로 돌아갔을 것
            if (unit is MonoBehaviour monoBehaviour && monoBehaviour.gameObject.activeSelf)
                monoBehaviour.gameObject.SetActive(false);
        }

        activeUnits.Clear();
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

        ProcessTotalHits(model, hitTarget);
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
        var hitEffect = await ObjectPoolManager.Instance.SpawnPoolableMono<TimedPoolableMono>
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
        DeactivateAllUnits();
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        BattleEventTriggerFactory.ReturnToPool(Model);
        BattleEventTriggerFactory.ReturnToPool(this);
    }
}
