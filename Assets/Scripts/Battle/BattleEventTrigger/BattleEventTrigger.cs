#pragma warning disable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEventTrigger
{
    protected BattleEventTriggerModel Model { get; private set; }
    protected int currentSendCount = 0;
 
    public void SetModel(BattleEventTriggerModel skillInfoValue)
    {
        Model = skillInfoValue;
    }

    public void Reset()
    {
        currentSendCount = 0;
        Model = null;
    }

    protected async UniTask<T> SpawnUnitAsync<T>(string prefabName, Vector3 position, Quaternion rotation) where T : PoolableMono, IBattleEventTriggerUnit
    {
        var unit = await ObjectPoolManager.Instance.SpawnPoolableMono<T>(prefabName, position, rotation);

        // 오브젝트 풀에서 재사용할 때 localScale값 리셋하므로 별도 리셋처리 필요 X
        if (Model.Scale > 0)
            unit.transform.localScale += unit.transform.localScale * Model.Scale;

        return unit;
    }

    private CharacterUnitModel GetTargetModel(Collider2D collider)
    {
        var targetModel = BattleSceneManager.Instance.GetCharacterModel(collider.gameObject.GetInstanceID());

        // 죽으면 타겟으로 감지안함.
        if (targetModel != null && targetModel.Hp <= 0)
            return null;

        return targetModel;
    }

    public virtual async UniTask Process() { }

    private void SendBattleEventToTarget(CharacterUnitModel targetModel, Collider2D hitTarget = null)
    {
        if (Model.BattleEventDatas.Count == 1)
        {
            SendBattleEvent(targetModel);
        }
        else
        {
            SendBattleEvents(targetModel);
        }

        if (hitTarget != null && !string.IsNullOrEmpty(Model.HitEffectPrefabName))
            OnSpawnHitEffect(hitTarget.transform.position).Forget();

        currentSendCount++;

        if (IsOverSendCount(currentSendCount))
            Complete();
    }

    private async UniTask OnSpawnHitEffect(Vector3 pos)
    {
        var hitEffect = await ObjectPoolManager.Instance.SpawnPoolableMono<TimedPoolableMono>
            (Model.HitEffectPrefabName, position: pos, Quaternion.identity);

        hitEffect.transform.localPosition += hitEffect.LocalPosOffset;
        hitEffect.Flip(hitEffect.transform.position.x < Model.Sender.Transform.position.x);
    }

    private void SendBattleEvent(CharacterUnitModel target)
    {
        var battleEventModel = Model.CreateBattleEventModel(target);
        target.EventProcessor.ReceiveBattleEvent(battleEventModel);
    }

    private void SendBattleEvents(CharacterUnitModel target)
    {
        var battleEventModels = Model.CreateBattleEventModels(target);
        foreach (var battleEventModel in battleEventModels)
        {
            target.EventProcessor.ReceiveBattleEvent(battleEventModel);
        }
    }

    private void Complete()
    {
        OnComplete();
        ReturnToPool();
    }

    protected virtual void OnComplete()
    {
    }

    private void ReturnToPool()
    {
        BattleEventTriggerFactory.ReturnToPool(Model);
        BattleEventTriggerFactory.ReturnToPool(this);
    }

    private bool IsOverSendCount(int count)
    {
        // 0이면 타격 제한이 없는 경우.
        if (Model.SendCount == 0)
            return false;

        return count >= Model.SendCount;
    }

    #region OnEvent
    protected void OnEventHit(Collider2D hitTarget)
    {
        if (Model == null)
            return;

        if (IsOverSendCount(currentSendCount))
            return;

        if (hitTarget == null)
            return;

        var targetModel = GetTargetModel(hitTarget);

        if (targetModel == null)
            return;

        if (targetModel.TeamTag != Model.TargetTeamTag)
            return;

        SendBattleEventToTarget(targetModel, hitTarget);
    }

    protected void OnEventSend(CharacterUnitModel targetModel)
    {
        if (targetModel == null)
            return;

        if (targetModel.TeamTag != Model.TargetTeamTag)
            return;

        SendBattleEventToTarget(targetModel);
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
    #endregion
}
